using JGUZDV.JobHost.Database;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Quartz;
using Quartz.Impl.Matchers;

namespace JGUZDV.JobHost
{
    internal class RegisterHost : IJob
    {
        private readonly IEnumerable<RegisterJob> _jobs;
        private readonly ISchedulerFactory _schedulerFactory;
        private readonly ILogger<RegisterHost> _logger;
        private readonly JobHostContext _dbContext;

        public RegisterHost(JobHostContext dbContext, 
            IEnumerable<RegisterJob> jobs, 
            ISchedulerFactory schedulerFactory,
            ILogger<RegisterHost> logger)
        {
            _jobs = jobs;
            _dbContext = dbContext;
            _schedulerFactory = schedulerFactory;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var scheduler = await _schedulerFactory.GetScheduler();
            await scheduler.PauseJobs(GroupMatcher<JobKey>.GroupEquals(JobKey.DefaultGroup));
            using var transaction = _dbContext.Database.BeginTransaction();
            try
            {
                
                var hostName = (string)context.JobDetail.JobDataMap[Constants.JobHostName];
                var host = await _dbContext.Hosts.FirstOrDefaultAsync(x => x.Name == hostName);
                
                if (host == null)
                {
                    // register the host
                    host = new Database.Entities.Host
                    {
                        MonitoringUrl = (string)context.JobDetail.JobDataMap[Constants.MonitoringUrl],
                        Name = hostName
                    };

                    _dbContext.Hosts.Add(host);
                    await _dbContext.SaveChangesAsync();
                }
                else
                {
                    // clean up old jobs
                    var jobs = await _dbContext
                        .Jobs
                        .AsNoTracking()
                        .Where(x => x.HostId == host.Id)
                        .ToListAsync();

                    var oldJobs = jobs
                        .Where(x => !_jobs.Any(y => y.JobName == x.Name))
                        .Select(x => x.Id)
                        .ToList();

                    await _dbContext.Jobs
                        .Where(x => oldJobs.Contains(x.Id))
                        .ExecuteDeleteAsync();
                }

                // register jobs
                foreach (var item in _jobs)
                {
                    await item.Execute(host, scheduler, _dbContext);
                }

                // register execute now polling job
                var jobDetail = JobBuilder
                .Create<ExecuteNowJob>()
                .WithIdentity(new JobKey(nameof(ExecuteNowJob)))
                .UsingJobData(Constants.JobHostName, host.Name)
                .Build();

                var schedule = (string)context.JobDetail.JobDataMap[Constants.ExecuteNowSchedule];
                var trigger = TriggerBuilder.Create()
                    .ForJob(jobDetail)
                    .WithCronSchedule(schedule)
                    .Build();

                await scheduler.ScheduleJob(jobDetail, trigger);
                transaction.Commit();   
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error during host initialization and register work");
                transaction.Rollback();
            }
            finally
            {
                await scheduler.ResumeJobs(GroupMatcher<JobKey>.GroupEquals(JobKey.DefaultGroup));
            }
        }
    }
}
