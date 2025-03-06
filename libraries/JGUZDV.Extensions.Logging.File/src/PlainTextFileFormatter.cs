using System.Diagnostics.CodeAnalysis;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

using static System.Text.Encoding;

namespace JGUZDV.Extensions.Logging.File;

internal sealed class PlainTextFileFormatter : FileFormatter, IDisposable
{
    private static readonly string _messagePadding = new string(' ', 5);
    private static readonly string _newLineWithMessagePadding = Environment.NewLine + _messagePadding;

    private static readonly string _newLine = Environment.NewLine;

    private readonly IDisposable? _optionsReloadToken;

    public PlainTextFileFormatter(IOptionsMonitor<PlainTextFileFormatterOptions> options)
    {
        ReloadLoggerOptions(options.CurrentValue);
        _optionsReloadToken = options.OnChange(ReloadLoggerOptions);
    }

    [MemberNotNull(nameof(FormatterOptions))]
    private void ReloadLoggerOptions(PlainTextFileFormatterOptions options)
    {
        FormatterOptions = options;
    }

    public void Dispose()
    {
        _optionsReloadToken?.Dispose();
    }

    public override FileFormatterOptions Options => FormatterOptions;
    internal PlainTextFileFormatterOptions FormatterOptions { get; set; }

    public override void Write<TState>(in LogEntry<TState> logEntry, IExternalScopeProvider? scopeProvider, Stream targetStream)
    {
        if (logEntry.State is BufferedLogRecord bufferedRecord)
        {
            string message = bufferedRecord.FormattedMessage ?? string.Empty;
            WriteInternal(null, targetStream, message, bufferedRecord.LogLevel, bufferedRecord.EventId.Id, bufferedRecord.Exception, logEntry.Category, bufferedRecord.Timestamp);
        }
        else
        {
            string message = logEntry.Formatter(logEntry.State, logEntry.Exception);
            if (logEntry.Exception == null && message == null)
            {
                return;
            }

            // We extract most of the work into a non-generic method to save code size. If this was left in the generic
            // method, we'd get generic specialization for all TState parameters, but that's unnecessary.
            WriteInternal(scopeProvider, targetStream, message, logEntry.LogLevel, logEntry.EventId.Id, logEntry.Exception?.ToString(), logEntry.Category, GetCurrentDateTime());
        }
    }

    private void WriteInternal(IExternalScopeProvider? scopeProvider, Stream targetStream, string message, LogLevel logLevel,
        int eventId, string? exception, string category, DateTimeOffset stamp)
    {
        using var writer = new StreamWriter(targetStream, UTF8, leaveOpen: true);

        string? timestamp = null;
        if (FormatterOptions.TimestampFormat is not null)
        {
            timestamp = stamp.ToString(FormatterOptions.TimestampFormat);
        }

        if (timestamp != null)
        {
            writer.Write(timestamp);
        }

        // Example:
        // 2024-12-12 [INF]: ConsoleApp.Program[10]
        //       Request received
        writer.WriteLine($"[{GetLogLevelString(logLevel)}]: {category}({eventId:0})");

        // scope information
        WriteScopeInformation(writer, scopeProvider);
        WriteMessage(writer, message);

        // Example:
        // System.InvalidOperationException
        //    at Namespace.Class.Function() in File:line X
        if (exception != null)
        {
            // exception message
            WriteMessage(writer, exception);
        }
    }

    private static void WriteMessage(TextWriter writer, string message)
    {
        if (!string.IsNullOrEmpty(message))
        {
            writer.Write(_messagePadding);
            writer.WriteLine(message.Replace(Environment.NewLine, _newLineWithMessagePadding));
        }
    }

    private DateTimeOffset GetCurrentDateTime()
    {
        return FormatterOptions.TimestampFormat != null
            ? (FormatterOptions.UseUtcTimestamp ? DateTimeOffset.UtcNow : DateTimeOffset.Now)
            : DateTimeOffset.MinValue;
    }

    private static string GetLogLevelString(LogLevel logLevel)
    {
        return logLevel switch
        {
            LogLevel.Trace => "TRC",
            LogLevel.Debug => "DBG",
            LogLevel.Information => "INF",
            LogLevel.Warning => "WRN",
            LogLevel.Error => "ERR",
            LogLevel.Critical => "CRT",
            
            _ => "___"
        };
    }

    private void WriteScopeInformation(TextWriter writer, IExternalScopeProvider? scopeProvider)
    {
        if (FormatterOptions.IncludeScopes && scopeProvider != null)
        {
            bool paddingNeeded = true;
            scopeProvider.ForEachScope((scope, state) =>
            {
                if (paddingNeeded)
                {
                    paddingNeeded = false;
                    state.Write(_messagePadding);
                }
                
                state.Write(" => ");
                state.Write(scope?.ToString() ?? "");
            }, writer);

            if (!paddingNeeded)
            {
                writer.WriteLine();
            }
        }
    }
}