using System.Collections.Concurrent;
using System.Runtime.Versioning;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace JGUZDV.Extensions.Logging.File;

/// <summary>
/// A provider of <see cref="FileLogger"/> instances.
/// </summary>
[UnsupportedOSPlatform("browser")]
[ProviderAlias("File")]
public class FileLoggerProvider : ILoggerProvider, ISupportExternalScope
{
    private readonly TimeProvider _timeProvider;
    private readonly IOptionsMonitor<FileLoggerOptions> _options;
    private readonly FileFormatter _formatter;

    private readonly ConcurrentDictionary<string, FileLogger> _loggers;


    private readonly FileLoggingProcessor _fileWriter;

    private readonly IDisposable? _optionsReloadToken;
    private IExternalScopeProvider _scopeProvider = NullExternalScopeProvider.Instance;

    /// <summary>
    /// Creates an instance of <see cref="FileLoggerProvider"/>.
    /// </summary>
    /// <param name="timeProvider">The time provider to use for timestamps.</param>
    /// <param name="options">The options to create <see cref="FileLogger"/> instances with.</param>
    /// <param name="formatter">Log formatter used for <see cref="FileLogger"/> instances.</param>
    public FileLoggerProvider(TimeProvider timeProvider, IOptionsMonitor<FileLoggerOptions> options, FileFormatter formatter)
    {
        _timeProvider = timeProvider;
        _options = options;
        _loggers = new ConcurrentDictionary<string, FileLogger>();

        _formatter = formatter;

        _fileWriter = new FileLoggingProcessor(options.CurrentValue, _timeProvider, _formatter.Options.FileExtension);
        //_optionsReloadToken = _options.OnChange(ReloadLoggerOptions);
    }


    // warning:  ReloadLoggerOptions can be called before the ctor completed,... before registering all of the state used in this method need to be initialized
    private void ReloadLoggerOptions(FileLoggerOptions options)
    {
        _fileWriter.ChangeChannel(options, _formatter.Options.FileExtension);
    }

    /// <inheritdoc />
    public ILogger CreateLogger(string name)
    {
        return _loggers.TryGetValue(name, out FileLogger? logger) ?
            logger :
            _loggers.GetOrAdd(name, new FileLogger(name, _fileWriter, _formatter, _scopeProvider, _options.CurrentValue));
    }


    /// <inheritdoc />
    public void Dispose()
    {
        _optionsReloadToken?.Dispose();
        _fileWriter.Dispose();
    }

    /// <inheritdoc />
    public void SetScopeProvider(IExternalScopeProvider scopeProvider)
    {
        _scopeProvider = scopeProvider;

        foreach (var logger in _loggers)
        {
            logger.Value.ScopeProvider = _scopeProvider;
        }
    }
}
