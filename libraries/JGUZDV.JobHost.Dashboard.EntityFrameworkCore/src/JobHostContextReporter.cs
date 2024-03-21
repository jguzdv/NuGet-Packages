using JGUZDV.JobHost.Shared;
using JGUZDV.JobHost.Shared.Model;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace JGUZDV.JobHost.Dashboard.EntityFrameworkCore
{
    /// <inheritdoc/>
    public class JobHostContextReporter : IJobExecutionManager
    {
        private readonly IOptions<JobReportOptions> _jobReportOptions;
        private readonly IDbContextFactory<JobHostContext> _dbContextFactory;

        /// <inheritdoc/>
        public JobHostContextReporter(IOptions<JobReportOptions> jobReportOptions, IDbContextFactory<JobHostContext> dbContextFactory)
        {
            _jobReportOptions = jobReportOptions;
            _dbContextFactory = dbContextFactory;
        }

        /// <inheritdoc/>
        public async Task RegisterHostAndJobsAsync(JobHostDescription jobHost, CancellationToken ct)
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync(ct);

            var host = await dbContext.Hosts.FirstOrDefaultAsync(x => x.Name == jobHost.HostName, ct);
            var jobs = await dbContext.Jobs.Where(x => x.Host!.Name == jobHost.HostName).ToListAsync(ct);

            if (host == null)
            {
                // register the host
                host = new Entities.Host
                {
                    MonitoringUrl = jobHost.MonitoringUrl,
                    Name = jobHost.HostName
                };

                dbContext.Hosts.Add(host);
            }
            else
            {
                // clean up old jobs
                var oldJobs = jobs
                    .Where(x => !jobHost.Jobs.Any(y => y.Name == x.Name))
                    .ToList();

                dbContext.Jobs.RemoveRange(oldJobs);
            }

            // register jobs

            foreach (var item in jobHost.Jobs)
            {
                var job = jobs.FirstOrDefault(x => x.Name == item.Name);

                if (job == null)
                {
                    job = new Entities.Job
                    {
                        LastExecutedAt = DateTimeOffset.MinValue,
                        NextExecutionAt = item.NextExecutionAt,
                        LastResult = "",
                        Schedule = item.CronSchedule,
                        Name = item.Name,
                        HostId = host.Id,
                    };

                    dbContext.Jobs.Add(job);
                }
                else
                {
                    job.Schedule = item.CronSchedule;
                }
            }

            await dbContext.SaveChangesAsync(ct);
        }

        /// <inheritdoc/>
        public async Task ReportJobExecutionAsync(JobExecutionReport jobReport)
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync();

            var name = jobReport.Name;
            var host = jobReport.Host;

            var result = jobReport.Failed ? Job.Success : Job.Error;
            await dbContext.Jobs
                .Where(x => x.Name == name && x.Host!.Name == host)
                .ExecuteUpdateAsync(entity => entity
                    .SetProperty(x => x.RunTime, jobReport.RunTime)
                    .SetProperty(x => x.NextExecutionAt, jobReport.NextFireTimeUtc)
                    .SetProperty(x => x.LastExecutedAt, jobReport.FireTimeUtc)
                    .SetProperty(x => x.LastResult, result)
                    .SetProperty(x => x.FailMessage, jobReport.FailMessage)
            );
        }

        /// <inheritdoc/>
        public async Task<List<Job>> GetPendingJobsAsync(CancellationToken ct)
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync(ct);

            var host = _jobReportOptions.Value.JobHostName;
            var jobs = await dbContext.Jobs
                .Where(x => x.Host!.Name == host && x.ShouldExecuteAt > x.LastExecutedAt)
                .Select(x => new Shared.Model.Job
                {
                    FailMessage = x.FailMessage,
                    HostId = x.HostId,
                    Id = x.Id,
                    LastExecutedAt = x.LastExecutedAt,
                    LastResult = x.LastResult,
                    Name = x.Name,
                    NextExecutionAt = x.NextExecutionAt,
                    RunTime = x.RunTime,
                    Schedule = x.Schedule,
                    ShouldExecuteAt = x.ShouldExecuteAt
                })
                .ToListAsync(ct);

            return jobs;
        }
    }
}
