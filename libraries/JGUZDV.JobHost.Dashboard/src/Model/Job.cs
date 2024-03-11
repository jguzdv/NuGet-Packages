namespace JGUZDV.JobHost.Dashboard.Model
{
    /// <summary>
    /// The Model class for a job
    /// </summary>
    public class Job
    {
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
        /// The flag for an instant execution of the job
        /// </summary>
        public required bool ShouldExecute { get; set; }

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
