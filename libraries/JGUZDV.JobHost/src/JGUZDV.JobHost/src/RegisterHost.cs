using System.Runtime.CompilerServices;

using JGUZDV.JobHost.Shared;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Quartz;
using Quartz.Impl.Matchers;

namespace JGUZDV.JobHost
{
    internal class RegisterHost : IJob, IHostedService
    {
        private readonly ISchedulerFactory _schedulerFactory;
        private readonly ILogger<RegisterHost> _logger;
        private readonly IJobExecutionManager _reporter;
        private readonly IOptions<JobReportOptions> _options;
        private readonly IServiceProvider _serviceProvider;

        public RegisterHost(IJobExecutionManager reporter,
            ISchedulerFactory schedulerFactory,
            ILogger<RegisterHost> logger,
            IOptions<JobReportOptions> options,
            IServiceProvider serviceProvider)
        {
            _reporter = reporter;
            _schedulerFactory = schedulerFactory;
            _logger = logger;
            _options = options;
            _serviceProvider = serviceProvider;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var scheduler = await _schedulerFactory.GetScheduler();
            await scheduler.PauseJobs(GroupMatcher<JobKey>.GroupEquals(JobKey.DefaultGroup));
            var hostName = (string)context.JobDetail.JobDataMap[Constants.JobHostName];

            try
            {
                var scope = _serviceProvider.CreateScope();
                var jobs = scope.ServiceProvider.GetServices<RegisterJob>();

                await _reporter.RegisterHostAndJobsAsync(new JobHostDescription
                {
                    HostName = hostName,
                    MonitoringUrl = (string)context.JobDetail.JobDataMap[Constants.MonitoringUrl],
                    Jobs = jobs.Select(x => new JobDescription
                    {
                        CronSchedule = x.CronSchedule,
                        Name = x.JobName,
                        NextExecutionAt = new CronExpression(x.CronSchedule).GetNextValidTimeAfter(DateTimeOffset.Now) ?? new() //TODO: TimeProvider
                    }).ToList()
                }, context.CancellationToken);

                // register quartz jobs
                foreach (var item in jobs)
                {
                    await item.Execute(hostName, scheduler, context.CancellationToken);
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

                await scheduler.ScheduleJob(jobDetail, trigger, context.CancellationToken);
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

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var scheduler = await _schedulerFactory.GetScheduler();
            var hostName = _options.Value.JobHostName;

            try
            {
                var scope = _serviceProvider.CreateScope();
                var jobs = scope.ServiceProvider.GetServices<RegisterJob>();

                await _reporter.RegisterHostAndJobsAsync(new JobHostDescription
                {
                    HostName = hostName,
                    MonitoringUrl = _options.Value.MonitoringUrl,
                    Jobs = jobs.Select(x => new JobDescription
                    {
                        CronSchedule = x.CronSchedule,
                        Name = x.JobName,
                        NextExecutionAt = new CronExpression(x.CronSchedule).GetNextValidTimeAfter(DateTimeOffset.Now) ?? new() //TODO: TimeProvider
                    }).ToList()
                }, cancellationToken);

                // register quartz jobs
                foreach (var item in jobs)
                {
                    await item.Execute(hostName, scheduler, cancellationToken);
                }

                // register execute now polling job
                var jobDetail = JobBuilder
                .Create<ExecuteNowJob>()
                .WithIdentity(new JobKey(nameof(ExecuteNowJob)))
                .UsingJobData(Constants.JobHostName, hostName)
                .Build();

                var schedule = _options.Value.ExecuteNowSchedule;
                var trigger = TriggerBuilder.Create()
                    .ForJob(jobDetail)
                    .WithCronSchedule(schedule)
                    .Build();

                await scheduler.ScheduleJob(jobDetail, trigger, cancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error during host initialization and register work");

            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
