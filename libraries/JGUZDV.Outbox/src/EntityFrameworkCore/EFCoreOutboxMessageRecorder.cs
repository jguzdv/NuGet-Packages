using JGUZDV.Outbox.Abstractions;

using Microsoft.EntityFrameworkCore;

namespace JGUZDV.Outbox.EntityFrameworkCore;

internal class EFCoreOutboxMessageRecorder<TDbContext> : IOutboxMessageRecorder 
    where TDbContext : DbContext
{
    private readonly TDbContext _dbContext;
    private readonly TimeProvider _timeProvider;

    public EFCoreOutboxMessageRecorder(
        TDbContext dbContext,
        TimeProvider timeProvider
        )
    {
        _dbContext = dbContext;
        _timeProvider = timeProvider;
    }

    public Task EnqueueMessageAsync(OutboxMessage message, CancellationToken ct)
    {
        message.RecordDate = _timeProvider.GetUtcNow();
        _dbContext.Set<OutboxMessage>().Add(message);

        return Task.CompletedTask;
    }

    public Task<int> SaveChangesAsync(CancellationToken ct)
        => _dbContext.SaveChangesAsync(ct);
}
