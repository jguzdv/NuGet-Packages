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

        public async Task<JobCollection> GetJobs()
        {
            var jobsByHost = await _dbContext.Jobs
                .Include(x => x.Host)
                .GroupBy(x => x.Host!)
                .Select(x => new
                {
                    Host = new Model.Host
                    {
                        MonitoringUrl = x.Key.MonitoringUrl,
                        Name = x.Key.Name
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
                .ToDictionaryAsync(x => x.Host, x => x.Jobs);

            return new JobCollection { JobsByHost = jobsByHost };
        }
    }
}
