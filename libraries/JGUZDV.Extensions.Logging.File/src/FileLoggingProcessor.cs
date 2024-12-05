using System.Diagnostics.CodeAnalysis;
using IO = System.IO;
using System.Threading.Channels;
using System.Timers;

namespace JGUZDV.Extensions.Logging.File;

/// <summary>
/// This will process log messages and write them to the file.
/// </summary>
internal class FileLoggingProcessor : IDisposable
{
    private readonly TimeProvider _timeProvider;
    private readonly string _fileNameExtension;

    private FileLoggerOptions _fileLoggerOptions;

    private Channel<Stream> _channel;
    private FileWriter? _currentWriter;
    private Task _fileProcessorTask;
        


    public FileLoggingProcessor(FileLoggerOptions options, TimeProvider timeProvider, string fileNameExtension)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fileNameExtension, nameof(fileNameExtension));
        _fileNameExtension = fileNameExtension.StartsWith(".") ? fileNameExtension : "." + fileNameExtension;

        _timeProvider = timeProvider;

        _fileLoggerOptions = options;

        InitializeChannel(options);
        InitializeProcessorTask(options);
    }


    // TODO Currently switched off / not called
    public void OnOptionsReload(FileLoggerOptions options)
    {
        _fileLoggerOptions = options;

        // TODO This won't work as the file that was previously written to is still open
        InitializeChannel(_fileLoggerOptions);
        InitializeProcessorTask(_fileLoggerOptions);
    }

    /// <summary>
    /// Make a first check for the file to write to, and set up the Consumer/Producer channel.
    /// </summary>
    /// <param name="options"></param>
    /// <exception cref="InvalidOperationException"></exception>
    [MemberNotNull(nameof(_channel))]
    private void InitializeChannel(FileLoggerOptions options)
    {
        var filePath = GetFilePath(options, 0);
        if(_currentWriter?.FilePath != filePath)
        {
            if (!FileIsWritable(filePath))
            {
                throw new InvalidOperationException($"The file '{filePath}' was not writable.");
            }
        }

        var oldChannel = _channel;

        _channel = Channel.CreateBounded<Stream>(
            new BoundedChannelOptions(options.MaxQueueLength)
        {
            FullMode = options.QueueFullMode switch
            {
                FileLoggerQueueFullMode.Wait => BoundedChannelFullMode.Wait,
                FileLoggerQueueFullMode.DropWrite => BoundedChannelFullMode.DropWrite,
                _ => BoundedChannelFullMode.Wait
            },
            SingleReader = true,
            SingleWriter = false
        });

        oldChannel?.Writer.Complete();

    }

    /// <summary>
    /// Initialize the processor task and a timer to restart the processor task if it faulted.
    /// </summary>
    /// <param name="options"></param>
    [MemberNotNull(nameof(_fileProcessorTask))]
    private void InitializeProcessorTask(FileLoggerOptions options)
    {
        _fileProcessorTask = ProcessMessagesAsync(_channel, options);
        _timeProvider.CreateTimer(CheckProcessorTask, this, TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1));
    }

    private void CheckProcessorTask(Object? state)
    {
        if (_fileProcessorTask.IsFaulted)
        {
            _fileProcessorTask = ProcessMessagesAsync(_channel, _fileLoggerOptions);
        }
    }


    private string GetFilePath(FileLoggerOptions options, int currentRollingFileNumber)
    {
        var outputDirectory = options.OutputDirectory;
        
        var fileName = GetFileNameFromPattern(options, currentRollingFileNumber);
        return IO.Path.Combine(outputDirectory ?? ".", fileName);
    }

    /// <summary>
    /// Compute filename for logfile.
    /// TODO: May lead to unexpected results, if the file name was changed while the 
    /// app is running (filenumber will not be reset).
    /// </summary>
    /// <param name="options"></param>
    /// <param name="currentRollingFileNumber"></param>
    /// <returns></returns>
    private string GetFileNameFromPattern(FileLoggerOptions options, int currentRollingFileNumber)
    {
        var fileNamePattern = options.FilenamePattern
            .Replace("{Date}", "{0}")
            .Replace("{Discriminator}", "{1}")
            .Replace("{Rolling}", "{2}");

        var refDate = options.PreferLocalTime
            ? _timeProvider.GetLocalNow()
            : _timeProvider.GetUtcNow();

        var rolling = options.AlwaysIncludeRollingNumber || currentRollingFileNumber != 0
            ? currentRollingFileNumber.ToString(options.RollingFileFormat)
            : string.Empty;

        var fileName = string.Format(
                fileNamePattern,
                _timeProvider.GetUtcNow().ToString(options.DateFormat),
                options.Discriminator,
                rolling
            )
            + options.FileExtension;

        return fileName;
    }


    private bool FileIsWritable(string filePath)
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
            using (FileStream fs = IO.File.Open(filePath, FileMode.OpenOrCreate, FileAccess.Write))
            {
                return fs.CanWrite;
            }
        }
        catch (Exception)
        {
            return false;
        }
    }

    private async Task ProcessMessagesAsync(Channel<Stream> channel, FileLoggerOptions options)
    {
        var refFileName = GetFileNameFromPattern(options, 0);

        try
        {
            var (filePath, rollingNumber, availableSpace) = GetNextRollingFile(options, 0);
            _currentWriter = CreateFileWriter(filePath, options.RollingFileSize);

            await foreach (var message in channel.Reader.ReadAllAsync())
            { 
                try
                {
                    await _currentWriter.WriteAsync(message);
                    
                    if (_currentWriter.IsFull)
                    {
                        (filePath, rollingNumber, availableSpace) = GetNextRollingFile(options, rollingNumber);
                        if(filePath != _currentWriter.FilePath)
                        {
                            _currentWriter.Dispose();
                            _currentWriter = CreateFileWriter(filePath, availableSpace);
                        }
                    }

                    // TODO: This would happen, if there's a time component in the filename.
                    //var nextRefFileName = GetFileNameFromPattern(options, 0);
                    //if (refFileName != nextRefFileName)
                    //{
                    //    refFileName = nextRefFileName;

                    //    await _currentWriter.DisposeAsync();
                    //    (_currentWriter, rollingNumber, availableSpace) = OpenNextFile(filePath, 0);
                    //}
                }
                catch
                {
                    // Do nothing. Prevents throwing an exception that kills the thread.
                }
                finally
                {
                    message.Dispose();
                }
            }
        }
        catch
        {
            // we're in a background process and are the logging instance, so we cannot do much here,
            // besides throwing an exception and hope someone listens.
            throw;
        }
        finally
        {
            _currentWriter?.Dispose();
        }


        (string filePath, int rollingNumber, long availableSpace) GetNextRollingFile(FileLoggerOptions options, int currentRollingFileNumber)
        {
            var filePath = GetFilePath(options, currentRollingFileNumber);
            var fileInfo = new FileInfo(filePath);
            var fileSize = fileInfo.Exists ? fileInfo.Length : 0L;

            if (fileSize >= options.RollingFileSize)
            {
                return GetNextRollingFile(options, ++currentRollingFileNumber);
            }

            return (filePath, currentRollingFileNumber, options.RollingFileSize - fileSize);
        }

        FileWriter CreateFileWriter(string filePath, long maxFileSize)
        {
            return new(filePath, maxFileSize);
        }
    }


    public void Dispose()
    {
        _channel.Writer.Complete();
    }

    internal void EnqueueMessage(MemoryStream message)
    {
        _channel.Writer.TryWrite(message);
    }
}

internal class FileWriter : IDisposable
{
    private readonly Stream _fileStream;
    private readonly long _maxFileSize;
    private long _fileSize;

    public string FilePath { get; }
    public bool IsFull => _fileSize >= _maxFileSize;

    public FileWriter(string filePath, long maxFileSize)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath, nameof(filePath));

        FilePath = filePath;
        _maxFileSize = maxFileSize;
        _fileStream = IO.File.Open(filePath, FileMode.Append, FileAccess.Write, FileShare.Read);
        _fileSize = _fileStream.Length;
    }


    public Task WriteAsync(Stream message)
    {
        message.Position = 0;
        _fileSize += message.Length;
        
        return message.CopyToAsync(_fileStream);
    }


    public void Dispose()
    {
        _fileStream.Dispose();
    }
}
