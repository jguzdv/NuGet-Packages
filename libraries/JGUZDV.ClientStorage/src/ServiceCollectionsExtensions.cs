using JGUZDV.ClientStorage.Defaults;
using JGUZDV.ClientStorage.Store;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace JGUZDV.ClientStorage;

/// <summary>
/// ServiceCollection extensions for registering client store with dependecies.
/// </summary>
public static class ServiceCollectionsExtensions
{
    /// <summary>
    /// Adds <see cref="ClientStore"/> without client side persistence. Uses <see cref="LifeCycleEvents"/> for <see cref="ILifeCycleEvents"/> 
    /// where the events must be triggered by the consumer
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddClientStoreWithNullStorage(this IServiceCollection services)
    {
        services.TryAddSingleton<IKeyValueStorage, NullStorage>();
        services.TryAddSingleton<ILifeCycleEvents, LifeCycleEvents>();
        services.AddMemoryCache();
        services.TryAddSingleton<ClientStore>();
        return services;
    }
}
