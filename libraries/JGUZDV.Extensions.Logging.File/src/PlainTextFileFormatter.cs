using System.Diagnostics.CodeAnalysis;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

using static System.Text.Encoding;

namespace JGUZDV.Extensions.Logging.File;

internal sealed class PlainTextFileFormatter : FileFormatter, IDisposable
{
    private const string LoglevelPadding = ": ";
    private static readonly string _messagePadding = new string(' ', GetLogLevelString(LogLevel.Information).Length + LoglevelPadding.Length);
    private static readonly string _newLineWithMessagePadding = Environment.NewLine + _messagePadding;

    private static readonly byte[] _newLine = UTF8.GetBytes(Environment.NewLine);

    private readonly IDisposable? _optionsReloadToken;

    public PlainTextFileFormatter(IOptionsMonitor<PlainTextFileFormatterOptions> options)
        : base(FileFormatterNames.Plain)
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
        string? timestamp = null;
        string? timestampFormat = FormatterOptions.TimestampFormat;
        if (timestampFormat != null)
        {
            timestamp = stamp.ToString(timestampFormat);
        }
        if (timestamp != null)
        {
            targetStream.Write(UTF8.GetBytes(timestamp));
        }

        targetStream.Write(UTF8.GetBytes(GetLogLevelString(logLevel)));

        // Example:
        // INF: ConsoleApp.Program[10]
        //       Request received

        // category and event id
        targetStream.Write(UTF8.GetBytes(LoglevelPadding));
        targetStream.Write(UTF8.GetBytes(category));
        targetStream.Write(UTF8.GetBytes("["));

        targetStream.Write(UTF8.GetBytes(eventId.ToString()));

        targetStream.Write(UTF8.GetBytes("]"));
        targetStream.Write(_newLine);

        // scope information
        WriteScopeInformation(targetStream, scopeProvider);
        WriteMessage(targetStream, message);

        // Example:
        // System.InvalidOperationException
        //    at Namespace.Class.Function() in File:line X
        if (exception != null)
        {
            // exception message
            WriteMessage(targetStream, exception);
        }
    }

    private static void WriteMessage(Stream targetStream, string message)
    {
        if (!string.IsNullOrEmpty(message))
        {
            
            targetStream.Write(UTF8.GetBytes(_messagePadding));
            WriteReplacing(targetStream, Environment.NewLine, _newLineWithMessagePadding, message);
            targetStream.Write(_newLine);
        }

        static void WriteReplacing(Stream targetStream, string oldValue, string newValue, string message)
        {
            string newMessage = message.Replace(oldValue, newValue);
            targetStream.Write(_newLine);
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

    private void WriteScopeInformation(Stream targetStream, IExternalScopeProvider? scopeProvider)
    {
        if (FormatterOptions.IncludeScopes && scopeProvider != null)
        {
            bool paddingNeeded = true;
            scopeProvider.ForEachScope((scope, state) =>
            {
                if (paddingNeeded)
                {
                    paddingNeeded = false;
                    state.Write(UTF8.GetBytes(_messagePadding));
                    state.Write(UTF8.GetBytes("=> "));
                }
                else
                {
                    state.Write(UTF8.GetBytes(" => "));
                }
                state.Write(UTF8.GetBytes(scope?.ToString() ?? ""));
            }, targetStream);

            if (!paddingNeeded)
            {
                targetStream.Write(_newLine);
            }
        }
    }
}