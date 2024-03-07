namespace JGUZDV.JobHost.Database.Entities
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
        /// The flag for an instant execution of the job
        /// </summary>
        public bool ShouldExecute { get; set; }
        
        /// <summary>
        /// The jobhost of this job
        /// </summary>
        public Host? Host { get; set; }
        
        /// <summary>
        /// The id of the jobhost of this job
        /// </summary>
        public int HostId { get; set; }
    }
}
