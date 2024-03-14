using JGUZDV.JobHost.Shared.Model;

namespace JGUZDV.JobHost.Shared
{
    /// <summary>
    /// The service for the dashboard. Loads and executes registered jobs.
    /// </summary>
    public interface IDashboardService
    {
        /// <summary>
        /// Loads the registered jobs and hosts 
        /// </summary>
        /// <returns></returns>
        public Task<JobCollection> GetJobs(CancellationToken ct);

        /// <summary>
        /// Marks the specified job to be executed
        /// </summary>
        /// <returns></returns>
        public Task ExecuteNow(int jobId, CancellationToken ct);
    }
}
