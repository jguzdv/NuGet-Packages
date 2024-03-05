using CommunityToolkit.Mvvm.ComponentModel;

using JGUZDV.ClientStorage.Store;
using JGUZDV.JobHost.Dashboard.Model;
using JGUZDV.JobHost.Dashboard.Services;

namespace JGUZDV.JobHost.Dashboard.Blazor
{
    public partial class DashboardState : ObservableObject
    {
        private readonly ClientStore _clientStore;
        private readonly IDashboardService _dashboardService;

        [ObservableProperty]
        private Task<JobCollection> _jobs;

        public DashboardState(IDashboardService dashboardService, ClientStore clientStore)
        {
            _dashboardService = dashboardService;
            _clientStore = clientStore;
            Initialize();
        }

        public Task ExecuteNow(int jobId)
        {
            return _dashboardService.ExecuteNow(jobId);
        }

        private void Initialize()
        {
            Jobs = _clientStore.GetOrLoad(
                 "SteveJobs",
                 new StoreOptions<JobCollection>
                 {
                     LoadFunc = (ct) => _dashboardService.GetSteveJobs(),
                     UsesBackgroundRefresh = true,
                     ValueExpiry = TimeSpan.FromSeconds(15)
                 });

            _clientStore.ValueChanged +=
                 (e) =>
                Jobs = _clientStore.Get<JobCollection>(e.Key);
        }
    }
}
