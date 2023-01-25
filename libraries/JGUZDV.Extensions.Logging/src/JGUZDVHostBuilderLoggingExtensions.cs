using JGUZDV.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Microsoft.Extensions.DependencyInjection;

public static class JGUZDVHostBuilderLoggingExtensions
{
    public static IHostBuilder UseJGUZDVLogging(this IHostBuilder hostBuilder)
        => hostBuilder.UseJGUZDVLogging(Constants.DefaultSectionName);

    public static IHostBuilder UseJGUZDVLogging(this IHostBuilder hostBuilder, string configSectionName)
        => hostBuilder.UseSerilog((ctx, logger) => logger.BuildSerilogLogger(ctx.HostingEnvironment, ctx.Configuration, configSectionName));

/*
#if NET7_0_OR_GREATER
    public static HostApplicationBuilder UseJGUZDVLogging(this HostApplicationBuilder applicationBuilder)
        => applicationBuilder.UseJGUZDVLogging(Constants.DefaultSectionName);

    public static HostApplicationBuilder UseJGUZDVLogging(this HostApplicationBuilder applicationBuilder, string configSectionName)
    {
        applicationBuilder
        applicationBuilder.UseSerilog((ctx, logger) => logger.BuildSerilogLogger(ctx.HostingEnvironment, ctx.Configuration, configSectionName));
    }

#endif
*/
}