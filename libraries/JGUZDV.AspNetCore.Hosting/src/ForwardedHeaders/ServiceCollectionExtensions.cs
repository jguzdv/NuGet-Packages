using JGUZDV.AspNetCore.Hosting.Extensions;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace JGUZDV.AspNetCore.Hosting.ForwardedHeaders;

public static class ServiceCollectionExtensions
{
    public static WebApplicationBuilder ConfigureForwardedHeaders(
        this WebApplicationBuilder builder,
        Action<ForwardedHeadersOptions>? options = null,
        string configSection = Constants.ConfigSections.ForwardedHeaders)
    {
        builder.Configuration.ValidateConfigSectionExists(configSection);
        builder.Services.ConfigureForwardedHeaders(builder.Configuration.GetSection(configSection), options);

        return builder;
    }

    public static IServiceCollection ConfigureForwardedHeaders(
        this IServiceCollection services,
        IConfiguration configSection,
        Action<ForwardedHeadersOptions>? configure = null)
    {
        services.Configure<ForwardedHeadersOptions>(opt =>
        {
            configSection.Bind(opt);
            configure?.Invoke(opt);
        });

        return services;
    }
}
