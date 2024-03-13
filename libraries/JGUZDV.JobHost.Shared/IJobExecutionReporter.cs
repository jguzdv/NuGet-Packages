using JGUZDV.JobHost.Shared.Model;

namespace JGUZDV.JobHost.Shared
{
    public interface IJobExecutionReporter
    {
        public Task RegisterHostAndJobsAsync(JobHostDescription jobHost);

        public Task ReportJobExecutionAsync(JobExecutionReport jobReport);

        public Task<List<Job>> GetPendingJobs();

        public Task RemoveFromPending(int jobId);
    }
}
