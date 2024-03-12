using JGUZDV.JobHost.Abstractions;

using Quartz;

namespace JGUZDV.JobHost
{
    internal class ExecuteNowJob : IJob
    {
        private readonly IJobExecutionReporterFactory _reporterFactory;
        private readonly ISchedulerFactory _schedulerFactory;

        public ExecuteNowJob(IJobExecutionReporterFactory reporterFactory, ISchedulerFactory schedulerFactory)
        {
            _reporterFactory = reporterFactory;
            _schedulerFactory = schedulerFactory;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var reporter = await _reporterFactory.CreateAsync();

            var jobs = await reporter.GetPendingJobs();

            var scheduler = await _schedulerFactory.GetScheduler();

            foreach (var job in jobs)
            {
                await reporter.RemoveFromPending(job.Id);

                var jobKey = new JobKey(job.Name);
                await scheduler.TriggerJob(jobKey);
            }
        }
    }
}
