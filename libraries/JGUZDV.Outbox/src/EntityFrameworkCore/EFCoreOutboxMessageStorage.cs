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
}