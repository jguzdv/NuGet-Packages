namespace JGUZDV.JobHost.Shared
{
    public class JobHostDescription
    {
        public List<JobDescription> Jobs { get; set; }
        public string HostName { get; set; }
        public string MonitoringUrl { get; set; }
    }

    public class JobDescription
    {
        public string CronSchedule { get; set; }
        public string Name { get; set; }
        public DateTimeOffset NextExecutionAt { get; set; }
    }
}
