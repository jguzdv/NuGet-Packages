using JGUZDV.Extensions.Logging.File;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for configuring logging.
/// </summary>
public static class JGUZDVLoggingExtensions
{
    private const string FileLoggerSectionName = $"{JGUZDV.AspNetCore.Logging.Constants.DefaultSectionName}:{JGUZDV.Extensions.Logging.File.Constants.FileProviderAlias}";

    /// <summary>
    /// Configures the web host to use the default logging configuration.
    /// </summary>
    public static WebApplicationBuilder UseJGUZDVLogging(this WebApplicationBuilder builder, ILogger? logger = null)
    {
        // TODO: Add HttpRequest logging?
        AddFileLogging(builder, logger);

        return builder;
    }

    private static void AddFileLogging(WebApplicationBuilder builder, ILogger? logger)
    {
        var isNotDevelopment = !builder.Environment.IsDevelopment();
        var hasFileSection = builder.Configuration.GetSection(FileLoggerSectionName).Exists();

        // We'll log a missing log file as an error in production.
        if (isNotDevelopment && !hasFileSection)
        {
            logger?.LogCritical(FileLoggerSectionName + " section is missing in the configuration. " +
                "JGUZDV Logging requires a file logging section in production.");
        }

        if (hasFileSection)
        {
            // In Production we'll blindly add the JSON file logger.
            if (isNotDevelopment)
            {
                builder.Logging.AddJsonFile();
            }
            else
            {
                // In Development we'll add the plain text file logger.
                builder.Logging.AddPlainTextFile();
            }


            builder.Services.PostConfigure<FileLoggerOptions>(configureOptions =>
            {
                if (string.IsNullOrWhiteSpace(configureOptions.OutputDirectory))
                {
                    throw new ArgumentException(
                        "No property OutputDirectory found in config section Logging:File. " +
                        "JGUZDV Logging needs a directory to store logfiles.");
                }

                // Add the application name to the output directory, so log files for
                // different apps will be written to different directories.
                configureOptions.OutputDirectory = Path.Combine(
                    configureOptions.OutputDirectory,
                    builder.Environment.ApplicationName);
            });
        }
    }
}

