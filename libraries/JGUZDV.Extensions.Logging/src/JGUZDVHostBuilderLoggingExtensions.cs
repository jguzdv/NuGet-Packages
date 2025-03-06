using JGUZDV.Extensions.Logging.File;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;


namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for configuring logging.
/// </summary>
public static class JGUZDVHostBuilderLoggingExtensions
{
    /// <summary>
    /// Configures the host builder to use the specified logging configuration.
    /// </summary>
    public static IHostBuilder UseJGUZDVLogging(this IHostBuilder hostBuilder, bool useJsonFormat = true)
    {
        if (useJsonFormat)
        {
            hostBuilder.ConfigureLogging((hostBuilderContext, loggingBuilder) =>
            {
                loggingBuilder.AddJsonFile();
            });
        }
        else
        {
            hostBuilder.ConfigureLogging((hostBuilderContext, loggingBuilder) =>
            {
                loggingBuilder.AddPlainTextFile();
            });
        }

        hostBuilder.ConfigureServices((hostContext, services) =>
        {
            services.PostConfigure<FileLoggerOptions>(configureOptions =>
            {
                if (string.IsNullOrWhiteSpace(configureOptions.OutputDirectory))
                {
                    throw new ArgumentException("No property OutputDirectory found in config section Logging:File. " +
                            "JGUZDV Logging needs a directory to store logfiles.");
                }

                // Add the application name to the output directory, so log files for
                // different apps will be written to different directories.
                configureOptions.OutputDirectory = Path.Combine(
                    configureOptions.OutputDirectory,
                    hostContext.HostingEnvironment.ApplicationName);
            });
        });


        return hostBuilder;
    }

}