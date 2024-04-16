using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace JGUZDV.JobHost.Dashboard.EntityFrameworkCore
{
    /// <summary>
    /// Extensions for the IHostBuilder
    /// </summary>
    public static class HostBuilderExtensions
    {

        /// <summary>
        /// Extends the host builder to configure job monitoring.
        /// The used <see cref="JobHostContextReporter"/> needs <see cref="IDbContextFactory{TContext}"/> of <see cref="JobHostContext"/> to work.
        /// The factory will NOT be registered for you
        /// </summary>
        /// <param name="builder">The host builder to extend.</param>
        /// <param name="section">Configuration section containing dashboard settings (default is <see cref="Constants.DefaultDashboardConfigSection"/>).</param>
        /// <returns>The extended host builder.</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static IHostBuilder UseJobHostContextReporting(this IHostBuilder builder,
            string section = Constants.DefaultDashboardConfigSection)
        {
            builder.UseJobReporting<JobHostContextReporter>(section);

            return builder;
        }

        /// <summary>
        /// Extends the host builder to configure job monitoring with specified parameters.
        /// The used <see cref="JobHostContextReporter"/> needs <see cref="IDbContextFactory{TContext}"/> of <see cref="JobHostContext"/> to work.
        /// The factory will NOT be registered for you
        /// </summary>
        /// <param name="builder">The host builder to extend.</param>
        /// <param name="configureOptions">Action that configures the <see cref="JobReportOptions"/></param>
        /// <returns>The extended host builder.</returns>
        public static IHostBuilder UseJobHostContextReporting(this IHostBuilder builder,
            Action<JobReportOptions> configureOptions)
        {
            builder.UseJobReporting<JobHostContextReporter>(configureOptions);

            return builder;
        }

        /// <summary>
        /// Extends the host builder to configure job monitoring.
        /// The used <see cref="JobHostContextReporter"/> needs <see cref="IDbContextFactory{TContext}"/> of <see cref="JobHostContext"/> to work.
        /// <see cref="IDbContextFactory{TContext}"/> of <see cref="JobHostContext"/> will be registered for you with the specified configure action
        /// </summary>
        /// <param name="builder">The host builder to extend.</param>
        /// <param name="configureDbContext">configure action for the DbContext factory</param>
        /// <param name="section">Configuration section containing dashboard settings (default is <see cref="Constants.DefaultDashboardConfigSection"/>).</param>
        /// <returns>The extended host builder.</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static IHostBuilder UseJobHostContextReporting(this IHostBuilder builder,
            Action<DbContextOptionsBuilder> configureDbContext,
            string section = Constants.DefaultDashboardConfigSection)
        {
            builder.ConfigureServices(services =>
            {
                services.AddDbContextFactory<JobHostContext>(configureDbContext);
            });

            builder.UseJobReporting<JobHostContextReporter>(section);

            return builder;
        }

        /// <summary>
        /// Extends the host builder to configure job monitoring with specified parameters.
        /// <see cref="IDbContextFactory{TContext}"/> of <see cref="JobHostContext"/> will be registered for you with the specified configure action
        /// </summary>
        /// <param name="configureDbContext">configure action for the DbContext factory</param>
        /// <param name="builder">The host builder to extend.</param>
        /// <param name="configureOptions">Action that configures the <see cref="JobReportOptions"/></param>
        /// <returns>The extended host builder.</returns>
        public static IHostBuilder UseJobHostContextReporting(this IHostBuilder builder,
            Action<DbContextOptionsBuilder> configureDbContext,
            Action<JobReportOptions> configureOptions)
        {
            builder.ConfigureServices(services =>
            {
                services.AddDbContextFactory<JobHostContext>(configureDbContext);
            });

            builder.UseJobReporting<JobHostContextReporter>(configureOptions);

            return builder;
        }
    }
}
