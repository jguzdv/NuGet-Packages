using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace JGUZDV.Blazor.Components.Authentication;

public static class DelegatedAuthenticationStateProviderExtensions
{
    public static IServiceCollection AddAuthenticationStateProvider<TFetchAuthenticationState>(
        this IServiceCollection services)
        where TFetchAuthenticationState : class, IFetchAuthenticationState
    {
        services.TryAddSingleton(sp => TimeProvider.System);
        services.TryAddScoped<IFetchAuthenticationState, TFetchAuthenticationState>();
        
        services.TryAddSingleton<DelegatedAuthenticationStateProvider>();

        return services;
    }
}
