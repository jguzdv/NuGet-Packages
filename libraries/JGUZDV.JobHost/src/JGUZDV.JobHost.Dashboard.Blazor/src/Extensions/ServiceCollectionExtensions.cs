using JGUZDV.Blazor.StateManagement;
using JGUZDV.ClientStorage;
using JGUZDV.JobHost.Dashboard.Blazor;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

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
        /// <param name="configureOptions">action that configures the <see cref="DashboardOptions"/> </param>
        /// <returns></returns>
        public static IServiceCollection AddDashboard(this IServiceCollection services, Action<DashboardOptions>? configureOptions = null)
        {
            services
                .AddOptions<DashboardOptions>()
                .Configure(x => configureOptions?.Invoke(x));

            services.AddClientStoreWithNullStorage();
            services.TryAddSingleton<DashboardState>();
            services.TryAddSingleton<IState<DashboardState>, State<DashboardState>>();

            return services;
        }

    }
}
