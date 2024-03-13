namespace JGUZDV.JobHost
{
    /// <summary>
    /// 
    /// </summary>
    public class JobReportOptions
    {
        /// <summary>
        /// name of the job host
        /// </summary>
        public string? JobHostName { get; set; }

        /// <summary>
        /// the url of external monitoring
        /// </summary>
        public string? MonitoringUrl { get; set; }

        /// <summary>
        /// the schedule for execute now option
        /// </summary>
        public string? ExecuteNowSchedule { get; set; }
    }
}
