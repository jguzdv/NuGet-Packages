using JGUZDV.JobHost.Abstractions.Model;
using JGUZDV.JobHost.Database;

using Microsoft.EntityFrameworkCore;

namespace JGUZDV.JobHost.Dashboard.Services
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
        public virtual async Task ExecuteNow(int jobId)
        {
            var context = await _dbContextFactory.CreateDbContextAsync();
            await context.Jobs
                .Where(x => x.Id == jobId)
                .ExecuteUpdateAsync(x => x.SetProperty(x => x.ShouldExecute, true));
        }

        /// <inheritdoc/>
        public virtual async Task<JobCollection> GetJobs()
        {
            var context = await _dbContextFactory.CreateDbContextAsync();
            var jobsByHost = await context.Jobs
                .Include(x => x.Host)
                .GroupBy(x => x.Host!)
                .Select(x => new
                {
                    Host = new Model.Host
                    {
                        MonitoringUrl = x.Key.MonitoringUrl,
                        Name = x.Key.Name,
                        Id = x.Key.Id
                    },
                    Jobs = x.Select(x => new Model.Job
                    {
                        Name = x.Name,
                        HostId = x.HostId,
                        Id = x.Id,
                        LastExecutedAt = x.LastExecutedAt,
                        LastResult = x.LastResult,
                        NextExecutionAt = x.NextExecutionAt,
                        Schedule = x.Schedule,
                        ShouldExecute = x.ShouldExecute,
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
