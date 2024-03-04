using System.ComponentModel;

namespace JGUZDV.JobHost.Dashboard.Model
{
    public class JobCollection 
    {
        private IReadOnlyDictionary<int, Host>? _hosts;
        private IReadOnlyDictionary<int, List<Job>>? _jobsByHost;

        public required IReadOnlyDictionary<int, Host> Hosts 
        { 
            get => _hosts!; 
            init 
            { 
                _hosts = value;
            } 
        }

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
