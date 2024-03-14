namespace JGUZDV.JobHost.Dashboard.EntityFrameworkCore.Entities
{
    /// <summary>
    /// The entity class for a job
    /// </summary>
    public class Job
    {
        /// <summary>
        /// The id of the job
        /// </summary>
        public int Id { get; set; }

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
        public DateTimeOffset? NextExecutionAt { get; set; }

        /// <summary>
        /// Indicates when the job should execute (regardless of schedule)
        /// </summary>
        public DateTimeOffset? ShouldExecuteAt { get; set; }

        /// <summary>
        /// The jobhost of this job
        /// </summary>
        public Host? Host { get; set; }

        /// <summary>
        /// The id of the jobhost of this job
        /// </summary>
        public int HostId { get; set; }

        /// <summary>
        /// The amount of time the job ran for
        /// </summary>
        public TimeSpan RunTime { get; set; }

        /// <summary>
        /// Cause of failure
        /// </summary>
        public string? FailMessage { get; set; }
    }
}
