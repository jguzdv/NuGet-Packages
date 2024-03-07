using CommunityToolkit.Mvvm.ComponentModel;

using JGUZDV.ClientStorage.Store;
using JGUZDV.JobHost.Dashboard.Model;
using JGUZDV.JobHost.Dashboard.Services;

using Microsoft.Extensions.Options;

namespace JGUZDV.JobHost.Dashboard.Blazor
{
    /// <summary>
    /// The state of the dashboard. 
    /// This class is used by the dashboard views and handles initialization and reloading of data.
    /// </summary>
    public partial class DashboardState : ObservableObject
    {
        private readonly ClientStore _clientStore;
        private readonly IDashboardService _dashboardService;
        private IOptions<DashboardOptions> _options;

        [ObservableProperty]
        private Task<JobCollection> _jobs;

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="dashboardService"></param>
        /// <param name="clientStore"></param>
        public DashboardState(IDashboardService dashboardService, ClientStore clientStore, IOptions<DashboardOptions> options)
        {
            _dashboardService = dashboardService;
            _clientStore = clientStore;
            _options = options;
            _jobs = Initialize();
        }

        /// <summary>
        /// Marks the speciefied job to be executed
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        public Task ExecuteNow(int jobId)
        {
            return _dashboardService.ExecuteNow(jobId);
        }

        private Task<JobCollection> Initialize()
        {
            var jobs = _clientStore.GetOrLoad(
                 "Jobhost:Jobs",
                 new StoreOptions<JobCollection>
                 {
                     LoadFunc = (ct) => _dashboardService.GetJobs(),
                     UsesBackgroundRefresh = true,
                     ValueExpiry = TimeSpan.FromSeconds(_options.Value.PollingInterval)
                 });

            _clientStore.ValueChanged +=
                 (e) =>
                Jobs = _clientStore.Get<JobCollection>(e.Key);
            return jobs;
        }
    }
}
