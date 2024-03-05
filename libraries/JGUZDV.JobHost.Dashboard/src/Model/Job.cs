namespace JGUZDV.JobHost.Dashboard.Model
{
    public class Job
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public required string Schedule { get; set; }

        public required DateTimeOffset LastExecutedAt { get; set; }
        public required string LastResult { get; set; }

        public required DateTimeOffset? NextExecutionAt { get; set; }

        public required bool ShouldExecute { get; set; }

        public required int HostId { get; set; }
    }
}
