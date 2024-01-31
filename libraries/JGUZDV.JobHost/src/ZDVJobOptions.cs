namespace JGUZDV.JobHost
{
    public class ZDVJobOptions
    {
        public JobOption[] JobOptions { get; set; }
    }

    public class JobOption
    {
        public string JobIdentifier { get; set; }

        public string CronSchedule { get; set; } 
    }

}
