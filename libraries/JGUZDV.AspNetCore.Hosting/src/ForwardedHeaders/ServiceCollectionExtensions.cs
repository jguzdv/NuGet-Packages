using JGUZDV.AspNetCore.Hosting.Extensions;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace JGUZDV.AspNetCore.Hosting.ForwardedHeaders;

/// <summary>
/// Extension methods for the ServiceCollection.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Configures the ForwardedHeaders middleware.
    /// </summary>
    public static WebApplicationBuilder ConfigureForwardedHeaders(
        this WebApplicationBuilder builder,
        Action<ForwardedHeadersOptions>? options = null,
        string configSection = Constants.ConfigSections.ForwardedHeaders)
    {
        builder.Configuration.ValidateConfigSectionExists(configSection);
        builder.Services.ConfigureForwardedHeaders(builder.Configuration.GetSection(configSection), options);

        return builder;
    }

    /// <summary>
    /// Configures the ForwardedHeaders middleware.
    /// </summary>
    public static IServiceCollection ConfigureForwardedHeaders(
        this IServiceCollection services,
        IConfiguration configSection,
        Action<ForwardedHeadersOptions>? configure = null)
    {
        services.Configure<ForwardedHeadersOptions>(opt =>
        {
            configSection.Bind(opt);

            var knownNetworks = configSection.GetSection("KnownNetworks").Get<string[]>();
            if (knownNetworks != null)
            {
                opt.KnownNetworks.Clear();

                foreach (var network in knownNetworks) {
                    opt.KnownNetworks.Add(IPNetwork.Parse(network));
                }
            }

            var knownProxies = configSection.GetSection("KnownProxies").Get<string[]>();
            if (knownProxies != null)
            {
                opt.KnownProxies.Clear();
                foreach (var proxy in knownProxies)
                {
                    opt.KnownProxies.Add(System.Net.IPAddress.Parse(proxy));
                }
            }

            configure?.Invoke(opt);
        });

        return services;
    }
}
