using Karambolo.Extensions.Logging.File;

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
    /// Adds file-based logging to the logging pipeline and registers file logger option configuration.
    /// </summary>
    /// <remarks>Adds the JSON file logger when the format is Json; otherwise adds the standard file logger.
    /// Registers ConfigureFileLoggerOptions for option binding.</remarks>
    /// <returns>The same logging builder instance.</returns>
    public static ILoggingBuilder AddJGUZDVLogging(
        this ILoggingBuilder loggingBuilder,
        IServiceCollection services,
        LoggingFileFormat fileFormat
    )
    {
        if (fileFormat == LoggingFileFormat.Json)
        {
            loggingBuilder.AddJsonFile();
        }
        else
        {
            loggingBuilder.AddFile();
        }

        services.ConfigureOptions<ConfigureFileLoggerOptions>();

        return loggingBuilder;
    }
}
