using System.Diagnostics.CodeAnalysis;
using IO = System.IO;
using System.Threading.Channels;

namespace JGUZDV.Extensions.Logging.File
{
    /// <summary>
    /// This will process log messages and write them to the file.
    /// </summary>
    internal class FileLoggingProcessor : IDisposable
    {
        private readonly TimeProvider _timeProvider;

        [NotNull]
        private Channel<Stream>? FileChannel { get; set; }


        private string _fileNameExtension;
        public string FileNameExtension {
            get => _fileNameExtension;
            set
            {
                ArgumentException.ThrowIfNullOrWhiteSpace(value, nameof(FileNameExtension));
                _fileNameExtension = value.StartsWith(".") ? value : "." + value;
            }
        }


        public FileLoggingProcessor(FileLoggerOptions options, TimeProvider timeProvider, string fileNameExtension)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(fileNameExtension, nameof(fileNameExtension));
            
            _timeProvider = timeProvider;

            InitializeChannel(options, fileNameExtension);
        }


        public void ChangeChannel(FileLoggerOptions options, string fileNameExtension)
            => InitializeChannel(options, fileNameExtension);

        private void InitializeChannel(FileLoggerOptions options, string fileNameExtension)
        {
            FileNameExtension = fileNameExtension;

            var filePath = GetFilePath(options, 0);
            if (!FileIsWritable(filePath))
            {
                throw new InvalidOperationException($"The file '{filePath}' was not writable.");
            }


            var oldChannel = FileChannel;

            FileChannel = Channel.CreateBounded<Stream>(
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
            _ = ProcessMessagesAsync(FileChannel, options);
        }


        private string GetFilePath(FileLoggerOptions options, int currentRollingFileNumber)
        {
            var outputDirectory = options.OutputDirectory;
            
            var fileName = GetFileNameFromPattern(options, currentRollingFileNumber);
            return IO.Path.Combine(outputDirectory ?? ".", fileName);
        }

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
                + _fileNameExtension;

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

            FileStream? currentFile = null;
            try
            {
                var (filePath, rollingNumber, availableSpace) = GetNextRollingFile(options, 0);
                currentFile = CreateFileStream(filePath);

                await foreach (var message in channel.Reader.ReadAllAsync())
                { 
                    try
                    {
                        message.Position = 0;
                        await message.CopyToAsync(currentFile);
                        availableSpace -= message.Length;

                        if (availableSpace < 0)
                        {
                            await currentFile.DisposeAsync();
                            (currentFile, rollingNumber, availableSpace) = OpenNextFile(filePath, rollingNumber);
                        }

                        // This would happen, if there's a time component in the filename.
                        var nextRefFileName = GetFileNameFromPattern(options, 0);
                        if (refFileName != nextRefFileName)
                        {
                            refFileName = nextRefFileName;

                            await currentFile.DisposeAsync();
                            (currentFile, rollingNumber, availableSpace) = OpenNextFile(filePath, 0);
                        }
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
                currentFile?.Dispose();
            }


            (string filePath, int rollingNumber, long availableSpace) GetNextRollingFile(FileLoggerOptions options, int currentRollingFileNumber)
            {
                var filePath = GetFilePath(options, currentRollingFileNumber);
                var fileInfo = new FileInfo(filePath);
                var fileSize = fileInfo.Exists ? fileInfo.Length : 0L;

                if (fileSize < options.RollingFileSize)
                {
                    return GetNextRollingFile(options, ++currentRollingFileNumber);
                }

                return (filePath, currentRollingFileNumber, options.RollingFileSize - fileSize);
            }

            FileStream CreateFileStream(string filePath)
            {
                return IO.File.Open(filePath, FileMode.Append, FileAccess.Write, FileShare.Read);
            }

            (FileStream stream, int rollingNumber, long availableSpace) OpenNextFile(string filePath, int rollingNumber)
            {
                var (nextFilePath, nextRollingNumber, nextAvailableSpace) = GetNextRollingFile(options, ++rollingNumber);
                return (CreateFileStream(nextFilePath), nextRollingNumber, nextAvailableSpace);
            }
        }


        public void Dispose()
        {
            FileChannel.Writer.Complete();
        }
    }
}
