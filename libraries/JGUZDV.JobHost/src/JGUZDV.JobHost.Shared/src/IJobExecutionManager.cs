using JGUZDV.JobHost.Shared.Model;

using Quartz;

namespace JGUZDV.JobHost.Shared
{
    /// <summary>
    /// The job execution manager. Allows the JobHost to communicate with external tools like the dashboard
    /// </summary>
    public interface IJobExecutionManager
    {
        /// <summary>
        /// Gets called during start up of the JobHost
        /// </summary>
        /// <param name="jobHost"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public Task RegisterHostAndJobsAsync(JobHostDescription jobHost, CancellationToken ct);

        /// <summary>
        /// Gets called each time a job was executed
        /// </summary>
        /// <param name="jobReport"></param>
        /// <returns></returns>
        public Task ReportJobExecutionAsync(JobExecutionReport jobReport);

        /// <summary>
        /// Gets jobs that are due to be executed now regardless of schedule. Gets called periodically based on the configured execute now schedule.
        /// </summary>
        /// <param name="ct"></param>
        /// <returns></returns>
        public Task<List<Job>> GetPendingJobsAsync(CancellationToken ct);

        /// <summary>
        /// Gets a job by its name
        /// </summary>
        /// <returns></returns>
        public Task<Job?> GetJob(string jobName, CancellationToken ct);


        /// <summary>
        /// Gets a job by its name from the job execution context 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task<Job?> GetJob(IJobExecutionContext context) => GetJob(context.JobDetail.Key.Name, context.CancellationToken);


        /// <summary>
        /// Gets a job by its type
        /// </summary>
        /// <typeparam name="TJob"></typeparam>
        /// <param name="ct"></param>
        /// <returns></returns>
        public Task<Job?> GetJob<TJob>(CancellationToken ct) where TJob : IJob => GetJob(typeof(TJob).Name, ct);

    }
}
