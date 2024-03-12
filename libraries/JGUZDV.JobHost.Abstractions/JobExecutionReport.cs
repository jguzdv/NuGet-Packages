namespace JGUZDV.JobHost.Abstractions
{
    public class JobExecutionReport
    {
        public required TimeSpan RunTime { get; set; }
        public required DateTimeOffset FireTimeUtc { get; set; }
        public required DateTimeOffset? NextFireTimeUtc { get; set; }

        public required string Name { get; set; }
        public required string Host { get; set; }

        public required bool Failed { get; set; }
        public required string? FailMessage { get; set; }
    }
}
