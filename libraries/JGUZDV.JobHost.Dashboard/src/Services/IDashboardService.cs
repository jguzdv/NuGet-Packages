using JGUZDV.JobHost.Dashboard.Model;

namespace JGUZDV.JobHost.Dashboard.Services
{
    public interface IDashboardService
    {
        public Task<JobCollection> GetSteveJobs();

        public Task ExecuteNow(int jobId);
    }
}
