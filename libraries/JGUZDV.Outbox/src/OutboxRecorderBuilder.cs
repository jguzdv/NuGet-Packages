using JGUZDV.Outbox.Abstractions;
using JGUZDV.Outbox.EntityFrameworkCore;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace JGUZDV.Outbox;

/// <summary>
/// Provides a builder for configuring outbox-recorder-related services and settings in the dependency injection container. 
/// </summary>
public class OutboxRecorderBuilder
{
    private readonly IServiceCollection _services;

    internal OutboxRecorderBuilder(IServiceCollection services)
    {
        _services = services;
        _services.TryAddSingleton((_) => TimeProvider.System);
    }

    
    /// <summary>
    /// Configures the outbox to use a custom implementation of IOutboxMessageRecorder for recording messages. This allows for flexibility in how messages are persisted, enabling the use of different storage mechanisms or custom logic for message recording.
    /// </summary>
    public OutboxRecorderBuilder UseMessageRecorder<TRecorder>() where TRecorder : class, IOutboxMessageRecorder
    {
        _services.AddScoped<IOutboxMessageRecorder, TRecorder>();
        return this;
    }
}
