
using JGUZDV.ClientStorage;
using JGUZDV.ClientStorage.Extensions;
using JGUZDV.ClientStorage.Store;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// ServiceCollection extensions for ClientStorage package.
/// </summary>
public static class ClientStorageExtensions
{
    /// <summary>
    /// Adds <see cref="ClientStore"/> without client side persistence.
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

    /// <summary>
    /// Adds <see cref="ClientStore"/> with localStorage persistence
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddClientStoreWASMDefaults(this IServiceCollection services)
    {
        services.AddSingleton<IKeyValueStorage, LocalStorage>();
        services.AddSingleton<ILifeCycleEvents, LifeCycleEvents>();
        services.AddSingleton<ClientStore>();
        return services;
    }
}
