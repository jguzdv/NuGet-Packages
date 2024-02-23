namespace JGUZDV.JobHost.Dashboard.Model
{
    public class Host
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public required string MonitoringUrl { get; set; }
    }
}
