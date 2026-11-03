using JGUZDV.JobHost.Shared;

using Quartz;

namespace JGUZDV.JobHost
{
    internal class ExecuteNowJob : IJob
    {
        private readonly IJobExecutionManager _reporter;
        private readonly ISchedulerFactory _schedulerFactory;

        public ExecuteNowJob(IJobExecutionManager reporter, ISchedulerFactory schedulerFactory)
        {
            _reporter = reporter;
            _schedulerFactory = schedulerFactory;
        }

        public async ValueTask Execute(IJobExecutionContext context, CancellationToken cancellationToken)
        {
            var scheduler = await _schedulerFactory.GetScheduler(cancellationToken);
            var jobs = await _reporter.GetPendingJobsAsync(cancellationToken);
            foreach (var job in jobs)
            {
                var jobKey = new JobKey(job.Name);
                await scheduler.TriggerJob(jobKey, new JobDataMap(), cancellationToken);
            }
        }
    }
}
