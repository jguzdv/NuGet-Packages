using System.Transactions;

using JGUZDV.Outbox.Abstractions;

using Microsoft.EntityFrameworkCore;

namespace JGUZDV.Outbox.EntityFrameworkCore;

internal class EFCoreOutboxMessageStorage<TDbContext> : EFCoreOutboxMessageRecorder<TDbContext>, IOutboxMessageStorage
    where TDbContext : DbContext
{
    private readonly TDbContext _dbContext;
    private readonly TimeProvider _timeProvider;

    public EFCoreOutboxMessageStorage(
        TDbContext dbContext,
        TimeProvider timeProvider
        ) : base(dbContext, timeProvider)
    {
        _dbContext = dbContext;
        _timeProvider = timeProvider;
    }


    public async Task<IEnumerable<OutboxMessage>> GetUnsentMessagesAsync(DateTimeOffset dueBefore, int batchSize, CancellationToken ct)
    {
        var messages = await _dbContext.Set<OutboxMessage>()
            .AsNoTracking()
            .Where(m => m.ProcessedDate == null && m.DueDate <= dueBefore)
            .OrderBy(m => m.DueDate)
            .Take(batchSize)
            .ToListAsync(ct);

        return messages;
    }


    public async Task MarkAsSentAsync(OutboxMessage message, CancellationToken ct)
    {
        await _dbContext.Set<OutboxMessage>()
            .Where(m => m.Id == message.Id)
            .ExecuteUpdateAsync(setters => 
                setters
                    .SetProperty(m => m.ProcessedDate, _timeProvider.GetUtcNow())
                    .SetProperty(m => m.HasBeenProcessed, true)
                ,
                ct);
    }

    public async Task MarkAsFailedAsync(OutboxMessage message, Exception exception, bool discard, CancellationToken ct)
    {
        message.FailCount++;
        message.Failures ??= new List<FailureInfo>();
        message.Failures.Add(new FailureInfo
        {
            FailedAt = _timeProvider.GetUtcNow(),
            StackTrace = exception.StackTrace,
            Reason = exception.Message
        });

        if (discard)
        {
            message.ProcessedDate = _timeProvider.GetUtcNow();
            message.HasBeenProcessed = false;
        }

        _dbContext.Update(message);
        await _dbContext.SaveChangesAsync(ct);
    }

    public async Task<IEnumerable<OutboxMessage>> GetMessagesAsync(MessageQuery query, CancellationToken ct)
    {
        var queryable = _dbContext.Set<OutboxMessage>().AsNoTracking().AsQueryable();

        if (query.DueBefore.HasValue)
        {
            queryable = queryable.Where(m => m.DueDate <= query.DueBefore.Value);
        }

        if (query.DueAfter.HasValue)
        {
            queryable = queryable.Where(m => m.DueDate >= query.DueAfter.Value);
        }

        if (!string.IsNullOrEmpty(query.MessageType))
        {
            queryable = queryable.Where(m => m.MessageType == query.MessageType);
        }

        if (!string.IsNullOrEmpty(query.Tags))
        {
            queryable = queryable.Where(m => m.Tags == query.Tags);
        }

        if (!query.IncludeProcessedMessages)
        {
            queryable = queryable.Where(m => m.HasBeenProcessed == null);
        }

        return await queryable.ToListAsync(ct);
    }

    public async Task RemoveMessageAsync(OutboxMessage message, CancellationToken ct)
    {
        await _dbContext.Set<OutboxMessage>()
            .Where(m => m.Id == message.Id)
            .ExecuteDeleteAsync(ct);
    }
}