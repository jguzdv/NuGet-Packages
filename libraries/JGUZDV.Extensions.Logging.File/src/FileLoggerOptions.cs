namespace JGUZDV.Extensions.Logging.File;

/// <summary>
/// Options for a <see cref="FileLogger"/>.
/// </summary>
public class FileLoggerOptions
{
    /// <summary>
    /// Gets or sets the name of the log message formatter to use.
    /// </summary>
    /// <value>
    /// The default value is <see langword="plain" />.
    /// </value>
    public string? FormatterName { get; set; }

    private FileLoggerQueueFullMode _queueFullMode = FileLoggerQueueFullMode.Wait;
    /// <summary>
    /// Gets or sets the desired console logger behavior when the queue becomes full.
    /// </summary>
    /// <value>
    /// The default value is <see langword="wait" />.
    /// </value>
    public FileLoggerQueueFullMode QueueFullMode
    {
        get => _queueFullMode;
        set
        {
            if (value != FileLoggerQueueFullMode.Wait && value != FileLoggerQueueFullMode.DropWrite)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "The FileLoggerQueueFullMode that has been provided is not valid.");
            }

            _queueFullMode = value;
        }
    }

    internal const int DefaultMaxQueueLengthValue = 2500;
    private int _maxQueuedMessages = DefaultMaxQueueLengthValue;

    /// <summary>
    /// Gets or sets the maximum number of enqueued messages.
    /// </summary>
    /// <value>
    /// The default value is 2500.
    /// </value>
    public int MaxQueueLength
    {
        get => _maxQueuedMessages;
        set
        {
            if (value <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "The MaxQueueLength needs to be positive.");
            }

            _maxQueuedMessages = value;
        }
    }

    /// <summary>
    /// Gets or sets the output directory, where log files will be written to.
    /// </summary>
    public string? OutputDirectory { get; set; }

    /// <summary>
    /// The discriminator may be included in the log-file name to e.g. distinguish between different machines.
    /// </summary>
    public string Discriminator { get; set; } = Environment.MachineName;

    /// <summary>
    /// The date format used in the log-file name. If you add hours or minutes here, the file will roll-over on every change.
    /// </summary>
    /// <value>
    /// The default value is yyyy-MM-dd.
    /// </value>
    public string DateFormat { get; set; } = "yyyy-MM-dd";


    /// <summary>
    /// Gets or sets a value indicating whether to use local time.
    /// </summary>
    /// <value>
    /// The default value is false
    /// </value>
    public bool PreferLocalTime { get; set; }


    /// <summary>
    /// The format used to distinguish rolling log files.
    /// </summary>
    /// <value>
    /// The default value is _000
    /// </value>
    public string RollingFileFormat { get; set; } = "_000";

    /// <summary>
    /// If set to <see langword="true" />, the rolling number will always be included in the log file name.
    /// </summary>
    /// <value>
    /// The default is false.
    /// </value>
    public bool AlwaysIncludeRollingNumber = false;

    /// <summary>
    /// The maximum acceptable file size before rolling the log file in byte.
    /// </summary>
    /// <value>
    /// The default value is 50Mb.
    /// </value>
    public long RollingFileSize { get; set; } = 50 * 1000 * 1024;


    /// <summary>
    /// Sets the filename pattern used to create the filenames.
    /// </summary>
    /// <remarks>
    /// Use {Date} {Discriminator} and {Rolling} to include the date, discriminator and rolling number in the filename.
    /// Do not append the File-Extension as it will be appended automatically.
    /// </remarks>
    public string FilenamePattern { get; set; } = "{Date}_{Discriminator}{Rolling}";
}