using JGUZDV.JobHost.Abstractions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace JGUZDV.JobHost.Database
{
    public class JobHostContextReporter : IJobExecutionReporter
    {
        private readonly IOptions<JobReportOptions> _jobReportOptions;
        private readonly IDbContextFactory<JobHostContext> _dbContextFactory;

        public JobHostContextReporter(IOptions<JobReportOptions> jobReportOptions, IDbContextFactory<JobHostContext> dbContextFactory)
        {
            _jobReportOptions = jobReportOptions;
            _dbContextFactory = dbContextFactory;
        }

        public async Task RegisterHostAndJobsAsync(JobHostDescription jobHost)
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync();

            var host = await dbContext.Hosts.FirstOrDefaultAsync(x => x.Name == jobHost.HostName);

            if (host == null)
            {
                // register the host
                host = new Database.Entities.Host
                {
                    MonitoringUrl = jobHost.MonitoringUrl,
                    Name = jobHost.HostName
                };

                dbContext.Hosts.Add(host);
                await dbContext.SaveChangesAsync();
            }
            else
            {
                // clean up old jobs
                var jobs = await dbContext.Jobs
                    .AsNoTracking()
                    .Where(x => x.HostId == host.Id)
                    .ToListAsync();

                var oldJobs = jobs
                    .Where(x => !jobHost.Jobs.Any(y => y.Name == x.Name))
                    .Select(x => x.Id)
                    .ToList();

                await dbContext.Jobs
                    .Where(x => oldJobs.Contains(x.Id))
                    .ExecuteDeleteAsync();

                await dbContext.SaveChangesAsync();
            }

            // register jobs
            foreach (var item in jobHost.Jobs)
            {
                var job = await dbContext.Jobs.FirstOrDefaultAsync(x => x.HostId == host.Id && x.Name == item.Name);

                if (job == null)
                {
                    job = new Database.Entities.Job
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
            }

            await dbContext.SaveChangesAsync();
        }

        public async Task ReportJobExecutionAsync(JobExecutionReport jobReport)
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync();

            var name = jobReport.Name;
            var host = jobReport.Host;

            var job = await dbContext.Jobs.FirstAsync(x => x.Name == name && x.Host!.Name == host);

            if (!jobReport.Failed)
            {
                job.LastResult = "success";
            }
            else
            {
                job.LastResult = "error";
                job.FailMessage = jobReport.FailMessage;
            }

            job.RunTime = jobReport.RunTime;
            job.NextExecutionAt = jobReport.NextFireTimeUtc;
            job.LastExecutedAt = jobReport.FireTimeUtc;
            job.ShouldExecute = false;

            await dbContext.SaveChangesAsync();
        }

        public async Task<List<Abstractions.Model.Job>> GetPendingJobs()
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync();

            var host = _jobReportOptions.Value.JobHostName;
            var jobs = await dbContext.Jobs
                .Where(x => x.Host.Name == host && x.ShouldExecute == true)
                .Select(x => new Abstractions.Model.Job
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
                    ShouldExecute = x.ShouldExecute
                })
                .ToListAsync();

            return jobs;
        }

        public async Task RemoveFromPending(int jobId)
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync();

            var entity = await dbContext.Jobs.FirstAsync(x => x.Id == jobId && x.Host.Name == _jobReportOptions.Value.JobHostName);
            entity.ShouldExecute = false;
            await dbContext.SaveChangesAsync();
        }
    }
}
