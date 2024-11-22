using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace JGUZDV.Extensions.Logging.File;

/// <summary>
/// Allows custom log messages formatting.
/// </summary>
public abstract class FileFormatter
{
    // <summary>
    /// Initializes a new instance of <see cref="FileFormatter"/>.
    /// </summary>
    /// <param name="name"></param>
    protected FileFormatter(string name)
    {
        ArgumentException.ThrowIfNullOrEmpty(name, nameof(name));

        Name = name;
    }

    /// <summary>
    /// Gets the name associated with the console log formatter.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Writes the log message to the specified TextWriter.
    /// </summary>
    /// <remarks>
    /// If the formatter wants to write colors to the console, it can do so by embedding ANSI color codes into the string.
    /// </remarks>
    /// <param name="logEntry">The log entry.</param>
    /// <param name="scopeProvider">The provider of scope data.</param>
    /// <param name="targetStream">The stream where we'll buffer bytes to be written.</param>
    /// <typeparam name="TState">The type of the object to be written.</typeparam>
    public abstract void Write<TState>(in LogEntry<TState> logEntry, IExternalScopeProvider? scopeProvider, Stream targetStream);
}
