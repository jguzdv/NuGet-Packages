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
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDashboard(this IServiceCollection services)
        {
            services.AddClientStoreWithNullStorage();
            services.AddSingleton<DashboardState>();
            services.AddSingleton<IState<DashboardState>, State<DashboardState>>();

            return services;
        }
    }
}
