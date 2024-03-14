using JGUZDV.JobHost.Shared;
using JGUZDV.JobHost.Shared.Model;

using Microsoft.EntityFrameworkCore;

namespace JGUZDV.JobHost.Dashboard.EntityFrameworkCore
{
    /// <inheritdoc/>
    public class DatabaseService : IDashboardService
    {
        private readonly IDbContextFactory<JobHostContext> _dbContextFactory;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="dbContextFactory"></param>
        public DatabaseService(IDbContextFactory<JobHostContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        /// <inheritdoc/>
        public virtual async Task ExecuteNow(int jobId, CancellationToken ct)
        {
            var context = await _dbContextFactory.CreateDbContextAsync(ct);
            await context.Jobs
                .Where(x => x.Id == jobId)
                .ExecuteUpdateAsync(x => x.SetProperty(x => x.ShouldExecuteAt, DateTimeOffset.Now), ct);
        }

        /// <inheritdoc/>
        public virtual async Task<JobCollection> GetJobs(CancellationToken ct)
        {
            var context = await _dbContextFactory.CreateDbContextAsync();
            var jobsByHost = await context.Jobs
                .Include(x => x.Host)
                .GroupBy(x => x.Host!)
                .Select(x => new
                {
                    Host = new Host
                    {
                        MonitoringUrl = x.Key.MonitoringUrl,
                        Name = x.Key.Name,
                        Id = x.Key.Id
                    },
                    Jobs = x.Select(x => new Job
                    {
                        Name = x.Name,
                        HostId = x.HostId,
                        Id = x.Id,
                        LastExecutedAt = x.LastExecutedAt,
                        LastResult = x.LastResult,
                        NextExecutionAt = x.NextExecutionAt,
                        Schedule = x.Schedule,
                        ShouldExecuteAt = x.ShouldExecuteAt,
                        FailMessage = x.FailMessage,
                        RunTime = x.RunTime
                    }).ToList()
                })
                .ToListAsync();

            return new JobCollection
            {
                JobsByHost = jobsByHost.ToDictionary(x => x.Host.Id, x => x.Jobs),
                Hosts = jobsByHost.ToDictionary(x => x.Host.Id, x => x.Host)
            };
        }
    }
}
