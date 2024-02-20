namespace JGUZDV.JobHost.Database.Entities
{
    public class Job
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Schedule { get; set; }

        public DateTimeOffset LastExecutedAt { get; set; }
        public string LastResult { get; set; }
        public string LastResultMessage { get; set; }

        public bool ShouldExecute { get; set; }

        public Host? Host { get; set; }
        public int HostId { get; set; }
    }
}
