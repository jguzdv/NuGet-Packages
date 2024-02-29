using CommunityToolkit.Mvvm.ComponentModel;

using JGUZDV.JobHost.Dashboard.Model;
using JGUZDV.JobHost.Dashboard.Services;

namespace JGUZDV.JobHost.Dashboard.Blazor
{
    public partial class DashboardState : ObservableObject
    {
        [ObservableProperty]
        private JobCollection _jobs;
        
        private IDashboardService _dashboardService;

        public DashboardState(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        public Task ExecuteNow(int jobId)
        {
            return _dashboardService.ExecuteNow(jobId);
        }
    }
}
