using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace JGUZDV.Extensions.Logging.File;

/// <summary>
/// Allows custom log messages formatting.
/// </summary>
public abstract class FileFormatter
{
    /// <summary>
    /// Writes the log message to the specified target stream.
    /// </summary>
    /// <param name="logEntry">The log entry.</param>
    /// <param name="scopeProvider">The provider of scope data.</param>
    /// <param name="targetStream">The stream where we'll buffer bytes to be written.</param>
    /// <typeparam name="TState">The type of the object to be written.</typeparam>
    public abstract void Write<TState>(in LogEntry<TState> logEntry, IExternalScopeProvider? scopeProvider, Stream targetStream);

    /// <summary>
    /// Gets the options associated with the file log formatter.
    /// </summary>
    public abstract FileFormatterOptions Options { get; }
}
