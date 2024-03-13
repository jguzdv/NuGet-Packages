using JGUZDV.JobHost.Abstractions;

using Quartz;

namespace JGUZDV.JobHost
{
    internal class ExecuteNowJob : IJob
    {
        private readonly IJobExecutionReporter _reporter;
        private readonly ISchedulerFactory _schedulerFactory;

        public ExecuteNowJob(IJobExecutionReporter reporter, ISchedulerFactory schedulerFactory)
        {
            _reporter = reporter;
            _schedulerFactory = schedulerFactory;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var jobs = await _reporter.GetPendingJobs();

            var scheduler = await _schedulerFactory.GetScheduler();

            foreach (var job in jobs)
            {
                await _reporter.RemoveFromPending(job.Id);

                var jobKey = new JobKey(job.Name);
                await scheduler.TriggerJob(jobKey);
            }
        }
    }
}
