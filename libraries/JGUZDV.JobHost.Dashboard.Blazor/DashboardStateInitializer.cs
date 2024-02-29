using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using JGUZDV.Blazor.StateManagement;
using JGUZDV.ClientStorage.Store;
using JGUZDV.JobHost.Dashboard.Model;
using JGUZDV.JobHost.Dashboard.Services;

using Microsoft.Extensions.Hosting;

namespace JGUZDV.JobHost.Dashboard.Blazor
{
    internal class DashboardStateInitializer : IHostedService
    {
        public DashboardStateInitializer(
            IState<DashboardState> dashboardState, 
            ClientStore clientStore, 
            IDashboardService dashboardService)
        {
            DashboardState = dashboardState;
            ClientStore = clientStore;
            DashboardService = dashboardService;
        }

        public IState<DashboardState> DashboardState { get; set; }

        public ClientStore ClientStore { get; set; }

        public IDashboardService DashboardService { get; set; }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            DashboardState.Value.Jobs = await ClientStore.GetOrLoad(
                "SteveJobs", 
                new StoreOptions<JobCollection> 
            { 
                LoadFunc = (ct) => DashboardService.GetSteveJobs(),
                UsesBackgroundRefresh = true,
                ValueExpiry = TimeSpan.FromSeconds(15)
            });

            ClientStore.ValueChanged += 
                async(e) => 
                DashboardState.Value.Jobs = await ClientStore.Get<JobCollection>(e.Key);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
