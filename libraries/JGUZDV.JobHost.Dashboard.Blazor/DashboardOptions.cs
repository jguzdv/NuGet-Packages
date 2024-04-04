namespace JGUZDV.JobHost.Dashboard.Blazor
{
    /// <summary>
    /// Options for the Dashboards.
    /// </summary>
    public class DashboardOptions
    {
        /// <summary>
        /// Number of seconds for the polling interval for the dashboard. After each intervall the dashboard reloads data.
        /// </summary>
        public int PollingInterval { get; set; } = 15;
    }
}