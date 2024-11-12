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
            var keys = scheduler.GetJobKeys(GroupMatcher<JobKey>.AnyGroup()).Result.ToList();

            Console.WriteLine("Available Jobs:\n");
            for (var i = 0; i < keys.Count; i++)
            {
                Console.WriteLine($"{i}: {keys[i].Name}");
            }

            JobKey? job = null;
            do
            {
                Console.WriteLine("\nType number of Job you want to Execute");

                if (int.TryParse(Console.ReadLine(), out int selectedJob) && (keys.Count - selectedJob > 0))
                    job = keys[selectedJob];

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
