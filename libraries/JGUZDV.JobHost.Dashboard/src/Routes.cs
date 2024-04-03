namespace JGUZDV.JobHost.Dashboard.Shared
{
    /// <summary>
    /// API Routes
    /// </summary>
    public static class Routes
    {
        /// <summary>
        /// Route for jobcollection endpoint
        /// </summary>
        public static string GetJobs { get; set; } = "jobs";

        /// <summary>
        /// Routetemplate for "execute now" endpoint
        /// </summary>
        public static string ExecuteNowTemplate = "jobs/{jobId}/execute";

        /// <summary>
        /// Builds route for "execute now" endpoint
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        public static string ExecuteNow(int jobId) => ExecuteNowTemplate.Replace("{jobId}", jobId.ToString());

    }
}
