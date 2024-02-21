namespace JGUZDV.JobHost.Dashboard
{
    public static class Routes
    {
        public static string GetJobs { get; set; } = "jobs";

        public static string ExecuteNowTemplate = "jobs/{jobId}/execute";
        public static string ExecuteNow(int jobId) => ExecuteNowTemplate.Replace("{jobId}", jobId.ToString());

    }
}
