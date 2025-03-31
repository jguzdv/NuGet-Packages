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
            Console.WriteLine(
                """

                ------- JobHost Debugging CLI -------
                Please select one of the registered Jobs by typing the corresponding number or type X to stop debugging.

                Resgistered Jobs:

                """);
            for (var i = 0; i < keys.Count; i++)
            {
                Console.WriteLine("{0,3}:\t{1}", i, keys[i].Name);
            }

            JobKey? job = null;
            do
            {
                Console.Write("\nSelect a job to execute: ");

                var input = Console.ReadLine();

                if (int.TryParse(input, out int selectedJob) && selectedJob < keys.Count)
                {
                    job = keys[selectedJob];
                }
                else
                {
                    if (input == null)
                        return;

                    if (input.Equals("x", StringComparison.OrdinalIgnoreCase))
                    {
                        _applicationLifetime.StopApplication();
                        return;
                    }
                    
                    Console.WriteLine("Invalid input! Please try again or stop debugging by typing X.");
                }

            } while (job == null);
            Console.WriteLine("\nStarting execution of {0}...", job.Name);

            await scheduler.TriggerJob(job);
            await scheduler.ResumeJob(job);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
