namespace JGUZDV.JobHost.Shared.Model
{
    /// <summary>
    /// The Model class for a job
    /// </summary>
    public class Job
    {
        /// <summary>
        /// Success result
        /// </summary>
        public static string Success => "success";

        /// <summary>
        /// Error result
        /// </summary>
        public static string Error => "error";

        /// <summary>
        /// The id of the job
        /// </summary>
        public required int Id { get; set; }

        /// <summary>
        /// The name of the job
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// The schedule for the job
        /// </summary>
        public required string Schedule { get; set; }

        /// <summary>
        /// The timestamp of last execution 
        /// </summary>
        public required DateTimeOffset LastExecutedAt { get; set; }

        /// <summary>
        /// The result of the last execution
        /// </summary>
        public required string LastResult { get; set; }

        /// <summary>
        /// The timestamp of the next execution
        /// </summary>
        public required DateTimeOffset? NextExecutionAt { get; set; }

        /// <summary>
        /// Indicates when the job should execute (regardless of schedule)
        /// </summary>
        public required DateTimeOffset? ShouldExecuteAt { get; set; }

        /// <summary>
        /// The id of the jobhost of this job
        /// </summary>
        public required int HostId { get; set; }

        /// <summary>
        /// The amount of time the job ran for
        /// </summary>
        public required TimeSpan RunTime { get; set; }

        /// <summary>
        /// Cause of failure
        /// </summary>
        public required string? FailMessage { get; set; }
    }
}
