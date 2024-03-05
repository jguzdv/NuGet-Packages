using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using JGUZDV.Blazor.StateManagement;
using JGUZDV.ClientStorage;
using JGUZDV.JobHost.Dashboard.Blazor;

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
        /// <returns></returns>
        public static IServiceCollection AddDashboard(this IServiceCollection services)
        {
            services.AddClientStoreWithNullStorage();
            services.AddSingleton<DashboardState>();
            services.AddSingleton<IState<DashboardState>, State<DashboardState>>();

            return services;
        }
    }
}
