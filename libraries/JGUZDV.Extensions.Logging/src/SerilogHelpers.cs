using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using System.Runtime.InteropServices;

namespace JGUZDV.Extensions.Logging;

public static class SerilogHelpers
{
    internal static void BuildSerilogLogger(this LoggerConfiguration logger, IHostEnvironment hostEnvironment, IConfiguration configuration, string configSectionName)
    {
        var config = configuration.GetSection(configSectionName);

        logger.Enrich.FromLogContext();
        logger.ApplyLogLevels(config, LogEventLevel.Warning);

        if (hostEnvironment.IsDevelopment())
        {
            logger.WriteTo.Logger(l =>
                l.ApplyLogLevels(config.GetSection("Debug"))
                    .WriteTo.Debug()
            );
        }

        logger.WriteTo.Logger(l =>
            l.ApplyLogLevels(config.GetSection("Console"))
                .WriteTo.Console()
        );

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            logger.WriteTo.Logger(l =>
            {
                var eventLogConfig = config.GetSection("EventLog");
                var source = eventLogConfig.GetValue<string?>("Source") ?? Constants.DefaultEventLogSource;

                l.ApplyLogLevels(eventLogConfig)
                    .WriteTo.EventLog(source);
            });
        }

        if (config.GetSection("File").Exists())
        {
            logger.WriteTo.Logger(l =>
            {
                var fileConfig = config.GetSection("File");

                var path = fileConfig.GetValue<string?>("Path") ?? string.Empty;
                var isolatePath = fileConfig.GetValue<bool?>("UseIsolatedPath") ?? true;
                var applicationName = fileConfig.GetValue<string?>("ApplicationName") ?? hostEnvironment.ApplicationName;
                var fileName = fileConfig.GetValue<string?>("FileName") ?? $"{Environment.MachineName}_.log";

                var useJson = fileName.EndsWith(".json", StringComparison.OrdinalIgnoreCase) || fileConfig.GetValue<bool?>("UseJson") != false;

                var logFileName = isolatePath
                    ? Path.Combine(path, applicationName, fileName)
                    : Path.Combine(path, fileName);

                l.ApplyLogLevels(fileConfig);

                if (useJson)
                {
                    l.WriteTo.File(new CompactJsonFormatter(), logFileName, 
                        rollingInterval: RollingInterval.Day, 
                        rollOnFileSizeLimit: true,
                        fileSizeLimitBytes: 5_000_000);
                }
                else
                    l.WriteTo.File(logFileName, 
                        rollingInterval: RollingInterval.Day,
                        rollOnFileSizeLimit: true,
                        fileSizeLimitBytes: 5_000_000);

            });
        }
    }

    private static LoggerConfiguration ApplyLogLevels(this LoggerConfiguration logger, IConfiguration config, LogEventLevel? fallbackDefaultLevel = null)
    {
        var logLevels = config.GetSection("LogLevel").Get<Dictionary<string, LogLevel>?>();
        if (logLevels?.TryGetValue("Default", out var defaultLogLevel) == true)
        {
            logger.MinimumLevel.Is(defaultLogLevel.ToSerilogLevel());
        }
        else if (fallbackDefaultLevel.HasValue)
        {
            logger.MinimumLevel.Is(fallbackDefaultLevel.Value);
        }

        if (logLevels != null)
        {
            foreach (var levelOverride in logLevels.Where(x => x.Key != "Default"))
                logger.MinimumLevel.Override(levelOverride.Key, levelOverride.Value.ToSerilogLevel());
        }

        return logger;
    }

    private static LogEventLevel ToSerilogLevel(this LogLevel logLevel)
        => logLevel switch
        {
            LogLevel.Trace => LogEventLevel.Verbose,
            LogLevel.Debug => LogEventLevel.Debug,
            LogLevel.Information => LogEventLevel.Information,
            LogLevel.Warning => LogEventLevel.Warning,
            LogLevel.Error => LogEventLevel.Error,
            //Critical and None will be "Fatal", since Serilog does not support "None"
            _ => LogEventLevel.Fatal
        };
}
