using Microsoft.Extensions.Hosting;

using Quartz;
using Quartz.Impl.Matchers;

namespace JGUZDV.JobHost
{
    internal class JobHostDebugService : IHostedService
    {
        private readonly IHostApplicationLifetime _applicationLifetime;
        private readonly ISchedulerFactory _schedulerFactory;
        private readonly IHostEnvironment _environment;

        public JobHostDebugService(IHostApplicationLifetime applicationLifetime,
            ISchedulerFactory schedulerFactory, IHostEnvironment environment)
        {
            _applicationLifetime = applicationLifetime;
            _schedulerFactory = schedulerFactory;
            _environment = environment;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _applicationLifetime.ApplicationStarted.Register(OnStarted);
            return Task.CompletedTask;
        }

        private void OnStarted()
        {
            var scheduler = _schedulerFactory.GetScheduler().Result;

            scheduler.PauseJobs(GroupMatcher<JobKey>.AnyGroup());
            var keys = scheduler.GetJobKeys(GroupMatcher<JobKey>.AnyGroup()).Result;

            Console.WriteLine("Available Jobs:");
            foreach (var key in keys)
            {
                Console.WriteLine(key.Name);
            }

            JobKey? job;
            do
            {
                Console.WriteLine("Type name of Job you want to Execute");
                job = keys.FirstOrDefault(k => k.Name.Equals(Console.ReadLine(), StringComparison.OrdinalIgnoreCase));
            } while (job == null);

            scheduler.TriggerJob(job);
            scheduler.ResumeJob(job);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
