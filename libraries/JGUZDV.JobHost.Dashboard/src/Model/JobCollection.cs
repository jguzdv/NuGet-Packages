namespace JGUZDV.JobHost.Dashboard.Model
{
    public class JobCollection
    {
        public required Dictionary<Host, List<Job>> JobsByHost { get; set; }
    }
}
