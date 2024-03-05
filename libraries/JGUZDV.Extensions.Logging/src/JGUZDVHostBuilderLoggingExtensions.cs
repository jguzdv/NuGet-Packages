using JGUZDV.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Serilog;

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
    public static IHostBuilder UseJGUZDVLogging(this IHostBuilder hostBuilder, string configSectionName)
        => hostBuilder.UseSerilog((ctx, logger) => logger.BuildSerilogLogger(ctx.HostingEnvironment, ctx.Configuration, configSectionName));
}