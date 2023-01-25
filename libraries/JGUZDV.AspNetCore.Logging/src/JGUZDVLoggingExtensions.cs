using JGUZDV.AspNetCore.Logging;
using Microsoft.AspNetCore.Builder;

namespace Microsoft.Extensions.DependencyInjection;

public static class JGUZDVLoggingExtensions
{
    public static WebApplicationBuilder UseJGUZDVLogging(this WebApplicationBuilder builder, string configSectionName = Constants.DefaultSectionName)
    {
        builder.Host.UseJGUZDVLogging(configSectionName);
        return builder;
    }
}

