using System.Net.Http.Headers;

using Duende.AccessTokenManagement.OpenIdConnect;

using JGUZDV.YARP.SimpleReverseProxy.Configuration;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

using Yarp.ReverseProxy.Configuration;
using Yarp.ReverseProxy.Transforms;
using Yarp.ReverseProxy.Transforms.Builder;

namespace JGUZDV.YARP.SimpleReverseProxy;

public static class SimpleReverseProxyExtensions
{
    /// <summary>
    /// Adds a single proxy for a single upstream target.
    /// </summary>
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
            services.AddOpenIdConnectAccessTokenManagement();

        services.AddSingleton<IProxyConfigProvider, SimpleReverseProxyConfigProvider>();
        services.AddReverseProxy()
            .AddTransforms(static ctx => ctx.AddRequestTransform(AddRequestLanguageHeader))
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
        var tokenResult = await ctx.HttpContext.GetUserAccessTokenAsync();

        if (tokenResult?.Succeeded == true && !string.IsNullOrWhiteSpace(tokenResult.Token.AccessToken))
        {
            ctx.ProxyRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenResult.Token.AccessToken);
            return;
        }
    }


    private static ValueTask AddRequestLanguageHeader(RequestTransformContext ctx)
    {
        var requestCultureFeature = ctx.HttpContext.Features.Get<IRequestCultureFeature>();

        if (requestCultureFeature is null)
        {
            return ValueTask.CompletedTask;
        }

        var languageHeader = ctx.ProxyRequest.Headers.AcceptLanguage;

        var languages = ctx.ProxyRequest.Headers.AcceptLanguage.ToList();
        languages.Insert(0, new(requestCultureFeature.RequestCulture.UICulture.Name, 1));

        ctx.ProxyRequest.Headers.AcceptLanguage.Clear();
        foreach(var headerValue in languages)
        {
            ctx.ProxyRequest.Headers.AcceptLanguage.Add(headerValue);
        }

        return ValueTask.CompletedTask;
    }

    public static ReverseProxyConventionBuilder MapSimpleReverseProxy(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapReverseProxy();
    }
}
