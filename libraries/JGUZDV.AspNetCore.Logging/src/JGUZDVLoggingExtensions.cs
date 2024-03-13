using Microsoft.AspNetCore.Builder;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for configuring logging.
/// </summary>
public static class JGUZDVLoggingExtensions
{
    /// <summary>
    /// Configures the web host to use the default logging configuration.
    /// </summary>
    public static WebApplicationBuilder UseJGUZDVLogging(this WebApplicationBuilder builder, 
        string configSectionName = JGUZDV.Extensions.Logging.Constants.DefaultSectionName, 
        bool writeToProviders = true)
    {
        builder.Host.UseJGUZDVLogging(configSectionName, writeToProviders);
        return builder;
    }
}

