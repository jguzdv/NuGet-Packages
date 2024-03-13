using JGUZDV.JobHost.Shared.Model;

namespace JGUZDV.JobHost.Dashboard.Services
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
        public Task<JobCollection> GetJobs();

        /// <summary>
        /// Marks the speciefied job to be executed
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        public Task ExecuteNow(int jobId);
    }
}
