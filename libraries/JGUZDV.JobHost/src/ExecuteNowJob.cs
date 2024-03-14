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

        public async Task Execute(IJobExecutionContext context)
        {
            var jobs = await _reporter.GetPendingJobsAsync(context.CancellationToken);

            var scheduler = await _schedulerFactory.GetScheduler(context.CancellationToken);

            foreach (var job in jobs)
            {
                var jobKey = new JobKey(job.Name);
                await scheduler.TriggerJob(jobKey, context.CancellationToken);
            }
        }
    }
}
