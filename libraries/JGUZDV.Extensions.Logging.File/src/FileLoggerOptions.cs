using Microsoft.Extensions.Logging;

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


    /* TODO:
     * Path
     * Filename-Pattern
     * Rollover by Size
     * Rollover by Time
     * Rollover by Date
     */
}