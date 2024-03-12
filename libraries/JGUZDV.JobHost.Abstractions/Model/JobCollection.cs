namespace JGUZDV.JobHost.Abstractions.Model
{
    /// <summary>
    /// Collection for hosts and their jobs.
    /// </summary>
    public class JobCollection
    {
        private IReadOnlyDictionary<int, Host>? _hosts;
        private IReadOnlyDictionary<int, List<Job>>? _jobsByHost;

        /// <summary>
        /// Dictionary for hosts with host.Id as key.
        /// </summary>
        public required IReadOnlyDictionary<int, Host> Hosts
        {
            get => _hosts!;
            init
            {
                _hosts = value;
            }
        }

        /// <summary>
        /// Dictionary for List of <see cref="Job"/> with job.HostId as key.
        /// </summary>
        public required IReadOnlyDictionary<int, List<Job>> JobsByHost
        {
            get => _jobsByHost!;
            init
            {
                _jobsByHost = value;
            }
        }
    }
}
