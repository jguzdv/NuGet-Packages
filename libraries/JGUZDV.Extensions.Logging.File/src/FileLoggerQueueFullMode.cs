namespace JGUZDV.Extensions.Logging.File;

/// <summary>
/// Describes the file logger behavior when the queue becomes full.
/// </summary>
public enum FileLoggerQueueFullMode
{
    /// <summary>
    /// Blocks the logging threads once the queue limit is reached.
    /// </summary>
    Wait,

    /// <summary>
    /// Drops new log messages when the queue is full.
    /// </summary>
    DropWrite
}