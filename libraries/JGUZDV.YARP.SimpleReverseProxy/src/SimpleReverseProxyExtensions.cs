using JGUZDV.YARP.SimpleReverseProxy;
using JGUZDV.YARP.SimpleReverseProxy.Configuration;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

using System.Net.Http.Headers;

using Yarp.ReverseProxy.Configuration;
using Yarp.ReverseProxy.Transforms;
using Yarp.ReverseProxy.Transforms.Builder;

namespace JGUZDV.YARP.SimpleReverseProxy;

public static class SimpleReverseProxyExtensions
{
    /// <summary>
    /// Adds a single proxy for a single upstream target.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns></returns>
    public static IServiceCollection AddSimpleReverseProxy(this IServiceCollection services,
        string? configSectionPath,
        Action<SimpleReverseProxyOptions>? configureOptions = null,
        bool enableAccessTokenManagement = true)
    {
        var optionBuilder = services.AddOptions<SimpleReverseProxyOptions>();

        if (configSectionPath != null)
            optionBuilder = optionBuilder.BindConfiguration(configSectionPath);

        if (configureOptions != null)
            optionBuilder = optionBuilder.Configure(configureOptions);


        if (enableAccessTokenManagement)
            services.AddAccessTokenManagement();

        services.AddSingleton<IProxyConfigProvider, SimpleReverseProxyConfigProvider>();
        services.AddReverseProxy()
            .AddTransforms(AddAccessTokenRequestTransform);

        return services;
    }

    private static void AddAccessTokenRequestTransform(TransformBuilderContext ctx)
    {
        if (ctx.Route.Metadata == null)
            return;

        if (!ctx.Route.Metadata.TryGetValue(nameof(SimpleProxyDefinition.UseAccessToken), out var addAT) || addAT == $"{false}")
            return;

        ctx.AddRequestTransform(AddBearerHeader);
    }

    private static async ValueTask AddBearerHeader(RequestTransformContext ctx)
    {
        var token = await ctx.HttpContext.GetUserAccessTokenAsync();

        if (string.IsNullOrWhiteSpace(token))
            return;

        ctx.ProxyRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    public static ReverseProxyConventionBuilder MapSimpleReverseProxy(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapReverseProxy();
    }
}
