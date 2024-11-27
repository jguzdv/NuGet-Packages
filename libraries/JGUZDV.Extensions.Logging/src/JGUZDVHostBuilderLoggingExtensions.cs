using JGUZDV.Extensions.Logging;
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
    /// Configures the host builder to use the default logging configuration.
    /// </summary>
    public static IHostBuilder UseJGUZDVLogging(this IHostBuilder hostBuilder)
        => hostBuilder.UseJGUZDVLogging(Constants.DefaultSectionName);

    /// <summary>
    /// Configures the host builder to use the specified logging configuration.
    /// </summary>
    public static IHostBuilder UseJGUZDVLogging(this IHostBuilder hostBuilder, string configSectionName, bool useJsonFormat = true)
    {
        hostBuilder.ConfigureServices((hostContext, services) =>
        {
            var fileSettings = hostContext.Configuration.GetSection(configSectionName + ":File");

            if (fileSettings == null)
            {
                throw new ArgumentException("No config section Logging:File found. JGUZDV Logging needs at least " +
                    "the property Logging:File:OutputDirectory to write logfiles.");
            }

            services.AddOptions<FileLoggerOptions>().Bind(fileSettings);

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
        });


        return hostBuilder;
    }

}