using JGUZDV.JobHost.Dashboard.Model;

namespace JGUZDV.JobHost.Dashboard.Services
{
    public interface IDashboardService
    {
        /// <summary>
        /// Loads the registered jobs and hosts 
        /// </summary>
        /// <returns></returns>
        public Task<JobCollection> GetSteveJobs();

        /// <summary>
        /// Marks the speciefied job to be executed
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        public Task ExecuteNow(int jobId);
    }
}
