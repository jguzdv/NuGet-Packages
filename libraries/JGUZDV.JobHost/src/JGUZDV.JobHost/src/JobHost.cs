using JGUZDV.JobHost.Shared;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Quartz;
using Quartz.Impl.Matchers;

namespace JGUZDV.JobHost
{
    /// <summary>
    /// Provides static methods for creating and configuring a host environment for job scheduling and management.
    /// </summary>
    public static class JobHost
    {
        /// <summary>
        /// Creates a host builder for configuring the job hosting environment.
        /// </summary>
        /// <param name="args"></param>
        /// <param name="configureWindowsService"></param>
        /// <param name="configureQuartz"></param>
        /// <returns></returns>
        public static HostApplicationBuilder CreateJobHostBuilder(string[] args, Action<WindowsServiceLifetimeOptions> configureWindowsService, Action<QuartzHostedServiceOptions> configureQuartz)
        {
            var builder = Host.CreateApplicationBuilder(args);
            builder.Services.AddQuartzHostedService(x =>
            {
                configureQuartz(x);
                x.AwaitApplicationStarted = true;
            });

            var disableJobSelection = builder.Configuration.GetValue<bool?>($"{Constants.DefaultConfigSection}:DisableDevelopmentJobSelection") ?? false;
            var isDevelopmentEnv = builder.Environment.IsDevelopment();
            if (isDevelopmentEnv && !disableJobSelection)
                builder.Services.AddHostedService<JobHostDebugService>();

            if (!isDevelopmentEnv)
                builder.Services.AddLogging(loggingBuilder =>
                {
                    loggingBuilder.AddJsonFile(options =>
                    {
                        if (string.IsNullOrWhiteSpace(options.OutputDirectory))
                            throw new ArgumentException("No property OutputDirectory found in config section Logging:File. " +
                                    "JGUZDV Logging needs a directory to store logfiles.");

                        options.OutputDirectory = Path.Combine(
                            options.OutputDirectory,
                            builder.Environment.ApplicationName);
                    });
                });

            builder.Services.AddWindowsService(configureWindowsService);

            return builder;
        }

        /// <summary>
        /// Extends the host builder to configure job monitoring.
        /// </summary>
        /// <param name="builder">The host builder to extend.</param>
        /// <param name="section">Configuration section containing dashboard settings (default is <see cref="Constants.DefaultDashboardConfigSection"/>).</param>
        /// <returns>The extended host builder.</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static HostApplicationBuilder UseJobReporting<T>(this IHostApplicationBuilder builder,
          string section = Constants.DefaultDashboardConfigSection) where T : class, IJobExecutionManager
        {
            builder.ConfigureReporting<T>(x =>
            {
                builder.Configuration.GetSection(section).Bind(x);
            });

            return (HostApplicationBuilder)builder;
        }

        private static void ConfigureReporting<T>(this IHostApplicationBuilder builder, Action<JobReportOptions> configureOptions) where T : class, IJobExecutionManager
        {
            builder.Services.Configure<JobReportOptions>(x =>
            {
                configureOptions(x);
            });

            builder.Services.TryAddSingleton<IJobExecutionManager, T>();

            builder.Properties[Constants.UsesDashboard] = true;

            builder.Services.AddQuartz(x =>
            {
                var matcher = GroupMatcher<JobKey>.GroupEquals(JobKey.DefaultGroup);
                x.AddJobListener<JobListener>(matcher);
            });

            builder.Services.AddHostedService<RegisterHost>();
        }

        /// <summary>
        /// Extends the host builder to configure job monitoring with specified parameters.
        /// </summary>
        /// <param name="builder">The host builder to extend.</param>
        /// <param name="configureOptions">Action that configures the <see cref="JobReportOptions"/></param>
        /// <returns>The extended host builder.</returns>
        public static HostApplicationBuilder UseJobReporting<T>(this IHostApplicationBuilder builder,
            Action<JobReportOptions> configureOptions) where T : class, IJobExecutionManager
        {
            builder.ConfigureReporting<T>(configureOptions);
            return (HostApplicationBuilder)builder;
        }

        /// <summary>
        /// Adds a hosted job to the host environment based on whether a dashboard is being used or not.
        /// </summary>
        public static HostApplicationBuilder AddHostedJob<TJob>(this IHostApplicationBuilder builder)
            where TJob : class, IJob
        {

            var schedule = builder.Configuration[$"{Constants.DefaultConfigSection}:{typeof(TJob).Name}"]
                ?? throw new InvalidOperationException($"'{Constants.DefaultConfigSection}:{typeof(TJob).Name}' could not be read from configuration.");
            if (schedule == "false")
            {
                return (HostApplicationBuilder)builder;
            }

            builder.AddHostedJob<TJob>(schedule);

            return (HostApplicationBuilder)builder;
        }

        /// <summary>
        /// Adds a hosted job to the host environment using a specified generic type 
        /// with its cron schedule retrieved from configuration.
        /// </summary>
        /// <param name="builder">The host builder to extend.</param>
        /// <param name="cronSchedule">The cron schedule for added Job.</param>
        /// <returns>The extended host builder.</returns>
        public static void AddHostedJob<TJob>(this IHostApplicationBuilder builder, string cronSchedule)
            where TJob : class, IJob
        {
            if (builder.Properties.ContainsKey(Constants.UsesDashboard) && builder.Properties[Constants.UsesDashboard] as bool? == true)
            {
                builder.Services.AddSingleton(x => new RegisterJob(typeof(TJob), cronSchedule));
            }
            else
            {
                builder.Services.AddQuartz(q =>
                {
                    var jobKey = new JobKey(typeof(TJob).Name);
                    q.AddJob<TJob>(jobKey, cfg =>
                    {
                        cfg.DisallowConcurrentExecution();
                    });
                    q.AddTrigger(opts => opts
                        .ForJob(jobKey)
                        .WithCronSchedule(cronSchedule));
                });
            }
        }
    }
}
