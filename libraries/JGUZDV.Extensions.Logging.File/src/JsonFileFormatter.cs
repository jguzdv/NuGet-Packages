using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace JGUZDV.Extensions.Logging.File;

internal sealed class JsonFileFormatter : FileFormatter, IDisposable
{
    private readonly IDisposable? _optionsReloadToken;

    public JsonFileFormatter(IOptionsMonitor<JsonFileFormatterOptions> options)
        : base(FileFormatterNames.Json)
    {
        ReloadLoggerOptions(options.CurrentValue);
        _optionsReloadToken = options.OnChange(ReloadLoggerOptions);
    }

    public override void Write<TState>(in LogEntry<TState> logEntry, IExternalScopeProvider? scopeProvider, Stream targetStream)
    {
        if (logEntry.State is BufferedLogRecord bufferedRecord)
        {
            string message = bufferedRecord.FormattedMessage ?? string.Empty;
            WriteInternal(null, targetStream, message, bufferedRecord.LogLevel, logEntry.Category, bufferedRecord.EventId.Id, bufferedRecord.Exception,
                bufferedRecord.Attributes.Count > 0, null, bufferedRecord.Attributes, bufferedRecord.Timestamp);
        }
        else
        {
            string message = logEntry.Formatter(logEntry.State, logEntry.Exception);
            if (logEntry.Exception == null && message == null)
            {
                return;
            }

            DateTimeOffset stamp = FormatterOptions.TimestampFormat != null
                ? (FormatterOptions.UseUtcTimestamp ? DateTimeOffset.UtcNow : DateTimeOffset.Now)
                : DateTimeOffset.MinValue;

            // We extract most of the work into a non-generic method to save code size. If this was left in the generic
            // method, we'd get generic specialization for all TState parameters, but that's unnecessary.
            WriteInternal(scopeProvider, targetStream, message, logEntry.LogLevel, logEntry.Category, logEntry.EventId.Id, logEntry.Exception?.ToString(),
                logEntry.State != null, logEntry.State?.ToString(), logEntry.State as IReadOnlyList<KeyValuePair<string, object?>>, stamp);
        }
    }

    private void WriteInternal(IExternalScopeProvider? scopeProvider, Stream targetStream, string? message, LogLevel logLevel,
        string category, int eventId, string? exception, bool hasState, string? stateMessage, IReadOnlyList<KeyValuePair<string, object?>>? stateProperties,
        DateTimeOffset stamp)
    {
        using (var writer = new Utf8JsonWriter(targetStream, FormatterOptions.JsonWriterOptions))
        {
            writer.WriteStartObject();
            var timestampFormat = FormatterOptions.TimestampFormat;
            if (timestampFormat != null)
            {
                writer.WriteString("Timestamp", stamp.ToString(timestampFormat));
            }
            writer.WriteNumber(nameof(LogEntry<object>.EventId), eventId);
            writer.WriteString(nameof(LogEntry<object>.LogLevel), GetLogLevelString(logLevel));
            writer.WriteString(nameof(LogEntry<object>.Category), category);
            writer.WriteString("Message", message);

            if (exception != null)
            {
                writer.WriteString(nameof(Exception), exception);
            }

            if (hasState)
            {
                writer.WriteStartObject(nameof(LogEntry<object>.State));
                writer.WriteString("Message", stateMessage);
                if (stateProperties != null)
                {
                    foreach (KeyValuePair<string, object?> item in stateProperties)
                    {
                        WriteItem(writer, item);
                    }
                }
                writer.WriteEndObject();
            }
            WriteScopeInformation(writer, scopeProvider);
            writer.WriteEndObject();

            writer.Flush();
            targetStream.Write(Encoding.UTF8.GetBytes(Environment.NewLine));
        }
    }

    private static string GetLogLevelString(LogLevel logLevel)
    {
        return logLevel switch
        {
            LogLevel.Trace => "Trace",
            LogLevel.Debug => "Debug",
            LogLevel.Information => "Information",
            LogLevel.Warning => "Warning",
            LogLevel.Error => "Error",
            LogLevel.Critical => "Critical",
            _ => throw new ArgumentOutOfRangeException(nameof(logLevel))
        };
    }

    private void WriteScopeInformation(Utf8JsonWriter writer, IExternalScopeProvider? scopeProvider)
    {
        if (FormatterOptions.IncludeScopes && scopeProvider != null)
        {
            writer.WriteStartArray("Scopes");
            scopeProvider.ForEachScope((scope, state) =>
            {
                if (scope is IEnumerable<KeyValuePair<string, object?>> scopeItems)
                {
                    state.WriteStartObject();
                    state.WriteString("Message", scope.ToString());
                    foreach (KeyValuePair<string, object?> item in scopeItems)
                    {
                        WriteItem(state, item);
                    }
                    state.WriteEndObject();
                }
                else
                {
                    state.WriteStringValue(ToInvariantString(scope));
                }
            }, writer);
            writer.WriteEndArray();
        }
    }

    private static void WriteItem(Utf8JsonWriter writer, KeyValuePair<string, object?> item)
    {
        var key = item.Key;
        switch (item.Value)
        {
            case bool boolValue:
                writer.WriteBoolean(key, boolValue);
                break;
            case byte byteValue:
                writer.WriteNumber(key, byteValue);
                break;
            case sbyte sbyteValue:
                writer.WriteNumber(key, sbyteValue);
                break;
            case char charValue:
                writer.WriteString(key, MemoryMarshal.CreateSpan(ref charValue, 1));
                break;
            case decimal decimalValue:
                writer.WriteNumber(key, decimalValue);
                break;
            case double doubleValue:
                writer.WriteNumber(key, doubleValue);
                break;
            case float floatValue:
                writer.WriteNumber(key, floatValue);
                break;
            case int intValue:
                writer.WriteNumber(key, intValue);
                break;
            case uint uintValue:
                writer.WriteNumber(key, uintValue);
                break;
            case long longValue:
                writer.WriteNumber(key, longValue);
                break;
            case ulong ulongValue:
                writer.WriteNumber(key, ulongValue);
                break;
            case short shortValue:
                writer.WriteNumber(key, shortValue);
                break;
            case ushort ushortValue:
                writer.WriteNumber(key, ushortValue);
                break;
            case null:
                writer.WriteNull(key);
                break;
            default:
                writer.WriteString(key, ToInvariantString(item.Value));
                break;
        }
    }

    private static string? ToInvariantString(object? obj) => Convert.ToString(obj, CultureInfo.InvariantCulture);

    public override FileFormatterOptions Options => FormatterOptions;

    internal JsonFileFormatterOptions FormatterOptions { get; set; }

    [MemberNotNull(nameof(FormatterOptions))]
    private void ReloadLoggerOptions(JsonFileFormatterOptions options)
    {
        FormatterOptions = options;
    }

    public void Dispose()
    {
        _optionsReloadToken?.Dispose();
    }
}