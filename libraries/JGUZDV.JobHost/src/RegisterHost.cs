using JGUZDV.JobHost.Abstractions;

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
        private readonly IJobExecutionReporter _reporter;

        public RegisterHost(IJobExecutionReporter reporter,
            IEnumerable<RegisterJob> jobs,
            ISchedulerFactory schedulerFactory,
            ILogger<RegisterHost> logger)
        {
            _jobs = jobs;
            _reporter = reporter;
            _schedulerFactory = schedulerFactory;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var scheduler = await _schedulerFactory.GetScheduler();
            await scheduler.PauseJobs(GroupMatcher<JobKey>.GroupEquals(JobKey.DefaultGroup));
            var hostName = (string)context.JobDetail.JobDataMap[Constants.JobHostName];

            try
            {
                await _reporter.RegisterHostAndJobsAsync(new JobHostDescription
                {
                    HostName = hostName,
                    MonitoringUrl = (string)context.JobDetail.JobDataMap[Constants.MonitoringUrl],
                    Jobs = _jobs.Select(x => new JobDescription
                    {
                        CronSchedule = x.CronSchedule,
                        Name = x.JobName,
                        NextExecutionAt = new CronExpression(x.CronSchedule).GetNextValidTimeAfter(DateTimeOffset.Now) ?? new() //TODO: TimeProvider
                    }).ToList()
                });

                // register quartz jobs
                foreach (var item in _jobs)
                {
                    await item.Execute(hostName, scheduler);
                }

                // register execute now polling job
                var jobDetail = JobBuilder
                .Create<ExecuteNowJob>()
                .WithIdentity(new JobKey(nameof(ExecuteNowJob)))
                .UsingJobData(Constants.JobHostName, hostName)
                .Build();

                var schedule = (string)context.JobDetail.JobDataMap[Constants.ExecuteNowSchedule];
                var trigger = TriggerBuilder.Create()
                    .ForJob(jobDetail)
                    .WithCronSchedule(schedule)
                    .Build();

                await scheduler.ScheduleJob(jobDetail, trigger);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error during host initialization and register work");
               
            }
            finally
            {
                await scheduler.ResumeJobs(GroupMatcher<JobKey>.GroupEquals(JobKey.DefaultGroup));
            }
        }
    }
}
