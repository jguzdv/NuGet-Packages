using System.Runtime.CompilerServices;
using System.Runtime.Versioning;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;

namespace JGUZDV.Extensions.Logging.File;

/// <summary>
/// A logger that writes messages into a file.
/// </summary>
[UnsupportedOSPlatform("browser")]
internal sealed class FileLogger : ILogger, IBufferedLogger
{
    private readonly string _name;
    private readonly FileLoggingProcessor _queueProcessor;

    internal FileLogger(
        string name,
        FileLoggingProcessor loggingProcessor,
        FileFormatter formatter,
        IExternalScopeProvider? scopeProvider,
        FileLoggerOptions options)
    {
        ArgumentException.ThrowIfNullOrEmpty(name, nameof(name));

        _name = name;
        _queueProcessor = loggingProcessor;
        Formatter = formatter;
        ScopeProvider = scopeProvider;
        Options = options;
    }

    internal FileFormatter Formatter { get; set; }
    internal IExternalScopeProvider? ScopeProvider { get; set; }
    internal FileLoggerOptions Options { get; set; }

    const int DefaultBufferSize = 1024;

    /// <inheritdoc />
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
        {
            return;
        }

        ArgumentNullException.ThrowIfNull(formatter, nameof(formatter));

        var message = new MemoryStream(DefaultBufferSize);
        LogEntry<TState> logEntry = new LogEntry<TState>(logLevel, _name, eventId, state, exception, formatter);
        Formatter.Write(in logEntry, ScopeProvider, message);

        _queueProcessor.EnqueueMessage(message);
    }

    /// <inheritdoc />
    public void LogRecords(IEnumerable<BufferedLogRecord> records)
    {
        ArgumentNullException.ThrowIfNull(records, nameof(records));

        foreach (var rec in records)
        {
            var message = new MemoryStream(DefaultBufferSize);

            var logEntry = new LogEntry<BufferedLogRecord>(rec.LogLevel, _name, rec.EventId, rec, null, static (s, _) => s.FormattedMessage ?? string.Empty);
            Formatter.Write(in logEntry, null, message);

            _queueProcessor.EnqueueMessage(message);
        }
    }

    /// <inheritdoc />
    public bool IsEnabled(LogLevel logLevel)
    {
        return logLevel != LogLevel.None;
    }

    /// <inheritdoc />
    public IDisposable BeginScope<TState>(TState state) where TState : notnull 
        => ScopeProvider?.Push(state) ?? NullScope.Instance;
}
