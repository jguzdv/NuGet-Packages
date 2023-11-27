using JGUZDV.ClientStorage;
using JGUZDV.ClientStorage.Defaults;
using JGUZDV.ClientStorage.Store;

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
    public static IServiceCollection AddClientStoreBlazorDefaults(this IServiceCollection services)
    {
        services.AddSingleton<IKeyValueStorage, LocalStorage>();
        services.AddSingleton<ILifeCycleEvents, BlazorLifeCycleEvents>();
        services.AddMemoryCache();
        services.AddSingleton<ClientStore>();
        return services;
    }
}
