using JGUZDV.AspNetCore.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using System.Runtime.InteropServices;

namespace Microsoft.Extensions.DependencyInjection;

public static class JGUZDVLoggingExtensions
{
    public static WebApplicationBuilder UseJGUZDVLogging(this WebApplicationBuilder builder, string configSectionName = Constants.DefaultSectionName)
    {
        builder.WebHost.UseSerilog((WebHostBuilderContext ctx, abc) => { })

        builder.Host.UseJGUZDVLogging(configSectionName);
        return builder;
    }
}

