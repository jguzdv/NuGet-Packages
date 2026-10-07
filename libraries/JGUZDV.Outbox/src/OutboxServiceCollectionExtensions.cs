using JGUZDV.Outbox;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Provides extension methods for the IServiceCollection interface to register outbox-related services and configurations in the dependency injection container.
/// </summary>
public static class OutboxServiceCollectionExtensions
{
    /// <summary>
    /// Adds the outbox recorder services to the dependency injection container and returns an OutboxRecorderBuilder for further configuration. 
    /// This is inteded to be used, when you create messages and want to store them in the outbox.
    /// </summary>
    public static OutboxRecorderBuilder AddOutboxRecorder(this IServiceCollection services)
    {
        return new OutboxRecorderBuilder(services);
    }

    /// <summary>
    /// Adds the outbox sender services to the dependency injection container and returns an OutboxSenderBuilder for further configuration.
    /// This is inteded to be used, when you want to send messages from the outbox.
    /// </summary>
    public static OutboxSenderBuilder AddOutboxSender(this IServiceCollection services)
    {
        return new OutboxSenderBuilder(services);
    }
}
