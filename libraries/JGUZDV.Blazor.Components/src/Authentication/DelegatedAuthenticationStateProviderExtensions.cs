﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace JGUZDV.Blazor.Components.Authentication;

public static class DelegatedAuthenticationStateProviderExtensions
{
    [Obsolete("Migrate to Components.Web and use PersistentAuthenticationStateProvider")]
    public static IServiceCollection AddAuthenticationStateProvider<TFetchAuthenticationState>(
        this IServiceCollection services)
        where TFetchAuthenticationState : class, IFetchAuthenticationState
    {
        services.TryAddSingleton(sp => TimeProvider.System);
        services.TryAddScoped<IFetchAuthenticationState, TFetchAuthenticationState>();
        
        services.TryAddSingleton<DelegatedAuthenticationStateProvider>();

        return services;
    }

    [Obsolete("Migrate to Components.Web and use PersistentAuthenticationStateProvider")]
    public static IServiceCollection AddDefaultAuthenticationStateProvider(this IServiceCollection services)
    {
        services.AddAuthenticationStateProvider<DefaultFetchClaimsPrincipal>();

        return services;
    }
}
