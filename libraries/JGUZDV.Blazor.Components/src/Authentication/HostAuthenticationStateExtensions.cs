using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace JGUZDV.Blazor.Components.Authentication;

public static class HostAuthenticationStateExtensions
{
    public static IServiceCollection AddHostAuthenticationState<TStateProvider, THostUser>(
        this IServiceCollection services,
        Action<HostAuthenticationOptions> configureOptions)
    where TStateProvider: HostAuthenticationStateProvider<THostUser>
    {
        services.AddOptions<HostAuthenticationOptions>()
            .Configure(configureOptions);

        services.AddScoped<HostAuthenticationStateProvider<THostUser>, TStateProvider>();
        services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<HostAuthenticationStateProvider<THostUser>>());

        return services;
    }
}
