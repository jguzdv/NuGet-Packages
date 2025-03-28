using Quartz;
using Quartz.Impl.Matchers;
using Quartz.Listener;

namespace JGUZDV.JobHost
{
    /// <summary>
    /// Quartz job listener. Used to listen for different job execution states and react accordingly.
    /// </summary>
    public class DevelopmentJobListener : JobListenerSupport
    {
        /// <summary>
        /// Name of the job listener used to distinguish between multiple ones. 
        /// </summary>
        public override string Name { get; }

        private readonly Func<IScheduler, List<JobKey>, Task> _jobSelection;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">Name of the listener.</param>
        /// <param name="jobSelection">Method to be called when job that is being listend to finishes execution.</param>
        public DevelopmentJobListener(string name, Func<IScheduler, List<JobKey>, Task> jobSelection)
        {
            Name = name;
            _jobSelection = jobSelection;
        }

        /// <summary>
        /// When a job that is being listend to by this listener finishes execution, it will be paused and an user specified method will get called.
        /// </summary>
        /// <param name="context">Job execution context.</param>
        /// <param name="jobException">Set if there war an Exception during execution.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns></returns>
        public override async Task JobWasExecuted(IJobExecutionContext context, JobExecutionException? jobException, CancellationToken cancellationToken = default)
        {
            await base.JobWasExecuted(context, jobException, cancellationToken);

            await context.Scheduler.PauseJob(context.JobDetail.Key);
            var keys = await context.Scheduler.GetJobKeys(GroupMatcher<JobKey>.AnyGroup());
            await _jobSelection.Invoke(context.Scheduler, keys.ToList());
        }
    }
}
