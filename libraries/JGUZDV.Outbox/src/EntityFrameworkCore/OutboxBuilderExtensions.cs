using JGUZDV.Outbox;
using JGUZDV.Outbox.EntityFrameworkCore;

using Microsoft.EntityFrameworkCore;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Provides extension methods for configuring Outbox with EFCore
/// </summary>
public static class OutboxBuilderExtensions
{
    extension(OutboxRecorderBuilder builder)
    {
        /// <summary>
        /// Configures the outbox to use a specific DbContext for recording messages. This method registers the EFCoreOutboxMessageRecorder as the implementation of IOutboxMessageRecorder, which will use the specified DbContext to persist outbox messages.
        /// </summary>
        public OutboxRecorderBuilder UseDbContextRecorder<TDbContext>() where TDbContext : DbContext
            => builder.UseMessageRecorder<EFCoreOutboxMessageRecorder<TDbContext>>();
    }

    extension(OutboxSenderBuilder builder) 
    {
        /// <summary>
        /// Configures the outbox to use a specific DbContext for storing messages. This method registers the EFCoreOutboxMessageStorage as the implementation of IOutboxMessageStorage, which will use the specified DbContext to manage the storage of outbox messages.
        /// </summary>
        public OutboxSenderBuilder UseDbContextMessageStorage<TDbContext>() where TDbContext : DbContext
            => builder.UseMessageStorage<EFCoreOutboxMessageStorage<TDbContext>>();

    }
}
