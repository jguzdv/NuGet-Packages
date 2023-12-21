using JGUZDV.ClientStorage;
using JGUZDV.ClientStorage.Defaults;
using JGUZDV.ClientStorage.Store;

using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// ServiceCollection extensions for ClientStorage package.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds <see cref="ClientStore"/> with localStorage persistence. Uses <see cref="BlazorLifeCycleEvents"/> for <see cref="ILifeCycleEvents"/> 
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddBrowserClientStore(this IServiceCollection services)
    {
        services.TryAddSingleton<IKeyValueStorage, LocalStorage>();
        services.TryAddSingleton<ILifeCycleEvents, BlazorLifeCycleEvents>();
        services.AddMemoryCache();
        services.TryAddSingleton<ClientStore>();
        return services;
    }
}
