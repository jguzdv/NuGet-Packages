namespace JGUZDV.JobHost.Dashboard.Model
{
    public class JobCollection
    {
        public Dictionary<int, Host> Hosts { get; set; }

        public required Dictionary<int, List<Job>> JobsByHost { get; set; }
    }
}
