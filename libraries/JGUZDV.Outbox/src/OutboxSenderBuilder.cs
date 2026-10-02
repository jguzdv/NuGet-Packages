using JGUZDV.Outbox.Abstractions;
using JGUZDV.Outbox.EntityFrameworkCore;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace JGUZDV.Outbox;

/// <summary>
/// Provides a builder for configuring outbox-sender-related services and settings in the dependency injection container.
/// </summary>
public class OutboxSenderBuilder
{
    private readonly IServiceCollection _services;

    internal OutboxSenderBuilder(IServiceCollection services)
    {
        _services = services;

        _services.TryAddSingleton((_) => TimeProvider.System);
        _services.TryAddScoped<OutboxWorker>();
    }


    /// <summary>
    /// Configures the outbox to use a custom implementation of IOutboxMessageStorage for storing messages. This allows for flexibility in how messages are stored, enabling the use of different storage mechanisms or custom logic for message management.
    /// </summary>
    public OutboxSenderBuilder UseMessageStorage<TStorage>() where TStorage : class, IOutboxMessageStorage
    {
        _services.AddScoped<IOutboxMessageStorage, TStorage>();
        return this;
    }

    /// <summary>
    /// Configures the outbox to use a custom implementation of IOutboxMessageSender for sending messages. This allows for flexibility in how messages are sent, enabling the use of different sending strategies or custom logic for message delivery.
    /// </summary>
    public OutboxSenderBuilder AddMessageSender<TSendingStrategy>() 
        where TSendingStrategy : class, IOutboxMessageSender
    {
        _services.TryAddEnumerable(ServiceDescriptor.Scoped<IOutboxMessageSender, TSendingStrategy>());
        return this;
    }

    /// <summary>
    /// Configures the outbox to use a custom implementation of IOutboxMessageSender for sending messages, along with specific options for that sender. This allows for flexibility in how messages are sent, enabling the use of different sending strategies or custom logic for message delivery, while also allowing for configuration of sender-specific settings.
    /// </summary>
    public OutboxSenderBuilder AddMessageSender<TSendingStrategy, TOptions>(Action<TOptions> configure)
        where TSendingStrategy : class, IOutboxMessageSender
        where TOptions : class
    {
        AddMessageSender<TSendingStrategy>();
        _services.AddOptions<TOptions>()
            .Configure(configure);

        return this;
    }

    /// <summary>
    /// Configures the outbox to use a custom implementation of IMessageFactory for creating messages of a specific type. This allows for flexibility in how messages are created, enabling the use of different factories or custom logic for message creation.
    /// </summary>
    public OutboxSenderBuilder AddMessageFactory<TFactory, TMessage>() where TFactory 
        : class, IMessageFactory<TMessage>
    {
        _services.AddScoped<IMessageFactory<TMessage>, TFactory>();
        return this;
    }
}