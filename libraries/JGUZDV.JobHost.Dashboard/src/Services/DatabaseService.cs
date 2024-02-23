using JGUZDV.JobHost.Dashboard.Model;
using JGUZDV.JobHost.Database;

using Microsoft.EntityFrameworkCore;

namespace JGUZDV.JobHost.Dashboard.Services
{
    public class DatabaseService : IDashboardService
    {
        private readonly JobHostContext _dbContext;

        public DatabaseService(JobHostContext dbContext)
        {
            _dbContext = dbContext;
        }

        public virtual Task ExecuteNow(int jobId)
        {
            return _dbContext.Jobs
                .Where(x => x.Id == jobId)
                .ExecuteUpdateAsync(x => x.SetProperty(x => x.ShouldExecute, true));
        }

        public virtual async Task<JobCollection> GetJobs()
        {
            var jobsByHost = await _dbContext.Jobs
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
                        LastResultMessage = x.LastResultMessage,
                        Schedule = x.Schedule,
                        ShouldExecute = x.ShouldExecute,
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
