namespace JGUZDV.JobHost.Database.Entities
{
    /// <summary>
    /// Entity class for a jobhosts
    /// </summary>
    public class Host
    {
        /// <summary>
        /// The id of the jobhost
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// THe Name of the jobhost
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// The Url for the monitoring
        /// </summary>
        public required string MonitoringUrl { get; set; } 
    }
}
