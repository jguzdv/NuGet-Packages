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

        private async void OnStarted()
        {
            var scheduler = await _schedulerFactory.GetScheduler();
            await scheduler.PauseJobs(GroupMatcher<JobKey>.AnyGroup());

            scheduler.ListenerManager.AddJobListener(new DevelopmentJobListener("DevelopmentJobListener", JobSelection), GroupMatcher<JobKey>.AnyGroup());

            var keys = scheduler.GetJobKeys(GroupMatcher<JobKey>.AnyGroup()).Result.ToList();
            await JobSelection(scheduler, keys);
        }

        private async Task JobSelection(IScheduler scheduler, List<JobKey> keys)
        {
            Console.WriteLine("Available Jobs:\n");
            for (var i = 0; i < keys.Count; i++)
            {
                Console.WriteLine($"{i}: {keys[i].Name}");
            }

            JobKey? job = null;
            do
            {
                Console.WriteLine("\nType number of Job you want to Execute");

                if (int.TryParse(Console.ReadLine(), out int selectedJob) && selectedJob < keys.Count)
                    job = keys[selectedJob];

            } while (job == null);

            await scheduler.TriggerJob(job);
            await scheduler.ResumeJob(job);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
