using JGUZDV.ClientStorage.Defaults;
using JGUZDV.ClientStorage.Store;
using Microsoft.Extensions.DependencyInjection;

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
        services.AddSingleton<IKeyValueStorage, NullStorage>();
        services.AddSingleton<ILifeCycleEvents, LifeCycleEvents>();
        services.AddSingleton<ClientStore>();
        return services;
    }
}
