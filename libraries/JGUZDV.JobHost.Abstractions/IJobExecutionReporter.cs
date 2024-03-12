using JGUZDV.JobHost.Abstractions.Model;

namespace JGUZDV.JobHost.Abstractions
{
    public interface IJobExecutionReporter
    {
        public Task RegisterHostAndJobsAsync(JobHostDescription jobHost);

        public Task ReportJobExecutionAsync(JobExecutionReport jobReport);

        public Task<List<Job>> GetPendingJobs();

        public Task RemoveFromPending(int jobId);
    }
}
