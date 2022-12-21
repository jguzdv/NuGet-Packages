using JGUZDV.AspNetCore.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;

namespace Microsoft.Extensions.DependencyInjection;

public static class LoggingExtensions
{
    public static WebApplicationBuilder UseZDVLogging(this WebApplicationBuilder builder, string configSectionName = Constants.DefaultSectionName)
    {
        builder.Host.UseZDVLogging(configSectionName);
        return builder;
    }

    public static IHostBuilder UseZDVLogging(this IHostBuilder hostBuilder)
        => hostBuilder.UseZDVLogging(Constants.DefaultSectionName);

    public static IHostBuilder UseZDVLogging(this IHostBuilder hostBuilder, string configSectionName)
    {
        hostBuilder.UseSerilog((ctx, logger) => BuildSerilogLogger(ctx, configSectionName, logger));
        return hostBuilder;
    }

    private static void BuildSerilogLogger(HostBuilderContext ctx, string configSectionName, LoggerConfiguration logger)
    {
        var config = ctx.Configuration.GetSection(configSectionName);

        logger.Enrich.FromLogContext();
        logger.ApplyLogLevels(config, LogEventLevel.Warning);

        if (ctx.HostingEnvironment.IsDevelopment())
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

        logger.WriteTo.Logger(l =>
        {
            var eventLogConfig = config.GetSection("EventLog");
            var source = eventLogConfig.GetValue<string?>("Source") ?? Constants.DefaultEventLogSource;

            l.ApplyLogLevels(eventLogConfig)
                .WriteTo.EventLog(source);
        });

        logger.WriteTo.Logger(l =>
        {
            var fileConfig = config.GetSection("File");

            var path = fileConfig.GetValue<string?>("Path");
            var isolatePath = fileConfig.GetValue<bool?>("UseIsolatedPath") ?? true;
            var applicationName = fileConfig.GetValue<string?>("ApplicationName") ?? ctx.HostingEnvironment.ApplicationName;
            var fileName = fileConfig.GetValue<string?>("FileName") ?? $"{Environment.MachineName}.log";

            var useJson = fileName.EndsWith(".json", StringComparison.OrdinalIgnoreCase) || fileConfig.GetValue<bool?>("UseJson") != false;
            if (useJson && !fileName.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
                fileName = Path.ChangeExtension(fileName, ".json");

            var logFileName = isolatePath
                ? Path.Combine(path, applicationName, fileName)
                : Path.Combine(path, fileName);

            l.ApplyLogLevels(fileConfig);

            if (useJson)
                l.WriteTo.File(new CompactJsonFormatter(), logFileName, rollingInterval: RollingInterval.Day);
            else
                l.WriteTo.File(logFileName, rollingInterval: RollingInterval.Day);

        });
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
        => (LogEventLevel)(int)logLevel;
}

