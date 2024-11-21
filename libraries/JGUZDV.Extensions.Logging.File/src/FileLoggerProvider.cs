using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;

namespace JGUZDV.Extensions.Logging.File;

/// <summary>
/// A provider of <see cref="FileLogger"/> instances.
/// </summary>
[UnsupportedOSPlatform("browser")]
[ProviderAlias("File")]
public partial class FileLoggerProvider : ILoggerProvider, ISupportExternalScope
{
    private readonly IOptionsMonitor<FileLoggerOptions> _options;
    private readonly ConcurrentDictionary<string, FileLogger> _loggers;
    private ConcurrentDictionary<string, FileFormatter> _formatters;
    private readonly FileLoggerProcessor _messageQueue;

    private readonly IDisposable? _optionsReloadToken;
    private IExternalScopeProvider _scopeProvider = NullExternalScopeProvider.Instance;

    /// <summary>
    /// Creates an instance of <see cref="FileLoggerProvider"/>.
    /// </summary>
    /// <param name="options">The options to create <see cref="FileLogger"/> instances with.</param>
    public FileLoggerProvider(IOptionsMonitor<FileLoggerOptions> options)
        : this(options, Array.Empty<FileFormatter>()) { }

    /// <summary>
    /// Creates an instance of <see cref="FileLoggerProvider"/>.
    /// </summary>
    /// <param name="options">The options to create <see cref="FileLogger"/> instances with.</param>
    /// <param name="formatters">Log formatters added for <see cref="FileLogger"/> instances.</param>
    public FileLoggerProvider(IOptionsMonitor<FileLoggerOptions> options, IEnumerable<FileFormatter>? formatters)
    {
        _options = options;
        _loggers = new ConcurrentDictionary<string, FileLogger>();
        SetFormatters(formatters);
        IConsole? console;
        IConsole? errorConsole;
        if (DoesConsoleSupportAnsi())
        {
            console = new AnsiLogConsole();
            errorConsole = new AnsiLogConsole(stdErr: true);
        }
        else
        {
            console = new AnsiParsingLogConsole();
            errorConsole = new AnsiParsingLogConsole(stdErr: true);
        }
        _messageQueue = new ConsoleLoggerProcessor(
            console,
            errorConsole,
            options.CurrentValue.QueueFullMode,
            options.CurrentValue.MaxQueueLength);

        ReloadLoggerOptions(options.CurrentValue);
        _optionsReloadToken = _options.OnChange(ReloadLoggerOptions);
    }


    [MemberNotNull(nameof(_formatters))]
    private void SetFormatters(IEnumerable<FileFormatter>? formatters = null)
    {
        var cd = new ConcurrentDictionary<string, FileFormatter>(StringComparer.OrdinalIgnoreCase);

        bool added = false;
        if (formatters != null)
        {
            foreach (FileFormatter formatter in formatters)
            {
                cd.TryAdd(formatter.Name, formatter);
                added = true;
            }
        }

        if (!added)
        {
            cd.TryAdd(FileFormatterNames.Plain, new PlainTextFileFormatter(new FormatterOptionsMonitor<PlainTextFileFormatterOptions>(new PlainTextFileFormatterOptions())));
            cd.TryAdd(FileFormatterNames.Json, new JsonFileFormatter(new FormatterOptionsMonitor<JsonFileFormatterOptions>(new JsonFileFormatterOptions())));
            //cd.TryAdd(FileFormatterNames.StructuredLog, new SystemdConsoleFormatter(new FormatterOptionsMonitor<ConsoleFormatterOptions>(new ConsoleFormatterOptions())));
        }

        _formatters = cd;
    }

    // warning:  ReloadLoggerOptions can be called before the ctor completed,... before registering all of the state used in this method need to be initialized
    private void ReloadLoggerOptions(FileLoggerOptions options)
    {
        if (options.FormatterName == null || !_formatters.TryGetValue(options.FormatterName, out FileFormatter? logFormatter))
        {
#pragma warning disable CS0618
            logFormatter = options.Format switch
            {
                FileLoggerFormat.Systemd => _formatters[FileFormatterNames.Systemd],
                _ => _formatters[FileFormatterNames.Simple],
            };
            if (options.FormatterName == null)
            {
                UpdateFormatterOptions(logFormatter, options);
            }
#pragma warning restore CS0618
        }

        _messageQueue.FullMode = options.QueueFullMode;
        _messageQueue.MaxQueueLength = options.MaxQueueLength;

        foreach (KeyValuePair<string, FileLogger> logger in _loggers)
        {
            logger.Value.Options = options;
            logger.Value.Formatter = logFormatter;
        }
    }

    /// <inheritdoc />
    public ILogger CreateLogger(string name)
    {
        if (_options.CurrentValue.FormatterName == null || !_formatters.TryGetValue(_options.CurrentValue.FormatterName, out FileFormatter? logFormatter))
        {
#pragma warning disable CS0618
            logFormatter = _options.CurrentValue.Format switch
            {
                ConsoleLoggerFormat.Systemd => _formatters[ConsoleFormatterNames.Systemd],
                _ => _formatters[ConsoleFormatterNames.Simple],
            };
#pragma warning restore CS0618

            if (_options.CurrentValue.FormatterName == null)
            {
                UpdateFormatterOptions(logFormatter, _options.CurrentValue);
            }
        }

        return _loggers.TryGetValue(name, out FileLogger? logger) ?
            logger :
            _loggers.GetOrAdd(name, new FileLogger(name, _messageQueue, logFormatter, _scopeProvider, _options.CurrentValue));
    }

#pragma warning disable CS0618
    private static void UpdateFormatterOptions(FileFormatter formatter, FileLoggerOptions deprecatedFromOptions)
    {
        // kept for deprecated apis:
        if (formatter is PlainTextFileFormatter defaultFormatter)
        {
            defaultFormatter.FormatterOptions = new SimpleConsoleFormatterOptions()
            {
                ColorBehavior = deprecatedFromOptions.DisableColors ? LoggerColorBehavior.Disabled : LoggerColorBehavior.Default,
                IncludeScopes = deprecatedFromOptions.IncludeScopes,
                TimestampFormat = deprecatedFromOptions.TimestampFormat,
                UseUtcTimestamp = deprecatedFromOptions.UseUtcTimestamp,
            };
        }
        else
        if (formatter is SystemdConsoleFormatter systemdFormatter)
        {
            systemdFormatter.FormatterOptions = new ConsoleFormatterOptions()
            {
                IncludeScopes = deprecatedFromOptions.IncludeScopes,
                TimestampFormat = deprecatedFromOptions.TimestampFormat,
                UseUtcTimestamp = deprecatedFromOptions.UseUtcTimestamp,
            };
        }
    }
#pragma warning restore CS0618

    /// <inheritdoc />
    public void Dispose()
    {
        _optionsReloadToken?.Dispose();
        _messageQueue.Dispose();
    }

    /// <inheritdoc />
    public void SetScopeProvider(IExternalScopeProvider scopeProvider)
    {
        _scopeProvider = scopeProvider;

        foreach (System.Collections.Generic.KeyValuePair<string, FileLogger> logger in _loggers)
        {
            logger.Value.ScopeProvider = _scopeProvider;
        }
    }
}