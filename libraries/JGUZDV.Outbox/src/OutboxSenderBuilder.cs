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

    public OutboxSenderBuilder UseMessageSender<TSendingStrategy>() 
        where TSendingStrategy : class, IOutboxMessageSender
    {
        _services.AddScoped<IOutboxMessageSender, TSendingStrategy>();
        return this;
    }

    public OutboxSenderBuilder UseMessageSender<TSendingStrategy, TOptions>(Action<TOptions> configure)
        where TSendingStrategy : class, IOutboxMessageSender
        where TOptions : class
    {
        _services.AddOptions<TOptions>()
            .Configure(configure);

        _services.AddScoped<IOutboxMessageSender, TSendingStrategy>();
        return this;
    }

    public OutboxSenderBuilder UseMessageFactory<TFactory, TMessage>() where TFactory 
        : class, IMessageFactory<TMessage>
    {
        _services.AddScoped<IMessageFactory<TMessage>, TFactory>();
        return this;
    }
}