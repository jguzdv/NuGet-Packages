using System.ComponentModel;

namespace JGUZDV.JobHost.Dashboard.Model
{
    public class JobCollection : INotifyPropertyChanged
    {
        private IReadOnlyDictionary<int, Host>? _hosts;
        private IReadOnlyDictionary<int, List<Job>>? _jobsByHost;

        public required IReadOnlyDictionary<int, Host> Hosts 
        { 
            get => _hosts!; 
            set 
            { 
                _hosts = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Hosts)));
            } 
        }

        public required IReadOnlyDictionary<int, List<Job>> JobsByHost 
        { 
            get => _jobsByHost!;
            set 
            { 
                _jobsByHost = value; 
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(JobsByHost)));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
