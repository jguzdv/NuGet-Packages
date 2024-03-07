using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using JGUZDV.Blazor.StateManagement;
using JGUZDV.ClientStorage;
using JGUZDV.JobHost.Dashboard.Blazor;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace JGUZDV.JobHost.Dashboard.Extensions 
{
    /// <summary>
    /// Extension class for the service collection
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds required services for the dashboard
        /// </summary>
        /// <param name="services"></param>
        /// <param name="pollingInterval">Number of seconds for the polling interval for the dashboard. After each intervall the dashboard reloads data. Default: 15s</param>
        /// <returns></returns>
        public static IServiceCollection AddDashboard(this IServiceCollection services, int pollingInterval = 15)
        {
            services.AddOptions<DashboardOptions>().Configure(x => x.PollingInterval = pollingInterval);
            services.AddClientStoreWithNullStorage();
            services.AddSingleton<DashboardState>();
            services.AddSingleton<IState<DashboardState>, State<DashboardState>>();

            return services;
        }

    }
}
