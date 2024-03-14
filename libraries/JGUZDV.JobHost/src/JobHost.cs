using JGUZDV.JobHost.Shared;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

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
        public static IHostBuilder CreateJobHostBuilder(string[] args, Action<WindowsServiceLifetimeOptions> configureWindowsService, Action<QuartzHostedServiceOptions> configureQuartz)
        {
            var builder = Host.CreateDefaultBuilder(args)
                            .ConfigureServices((ctx, services) =>
                            {
                                services.AddQuartzHostedService(x =>
                                {
                                    configureQuartz(x);
                                    x.AwaitApplicationStarted = true;
                                });
                            })
                           .UseJGUZDVLogging()
                           .UseWindowsService(configureWindowsService);

            return builder;
        }

        /// <summary>
        /// Extends the host builder to configure job monitoring.
        /// </summary>
        /// <param name="builder">The host builder to extend.</param>
        /// <param name="section">Configuration section containing dashboard settings (default is <see cref="Constants.DefaultDashboardConfigSection"/>).</param>
        /// <returns>The extended host builder.</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static IHostBuilder UseJobReporting<T>(this IHostBuilder builder,
          string section = Constants.DefaultDashboardConfigSection) where T : class, IJobExecutionManager
        {
            var jobHostName = "";
            var monitoringUrl = "";
            var executeNowSchedule = "";

            builder.ConfigureServices((ctx, services) =>
            {
                jobHostName = ctx.Configuration[$"{section}:{Constants.JobHostName}"]
                     ?? throw new InvalidOperationException($"'{section}:{Constants.JobHostName}' could not be read from configuration.");

                monitoringUrl = ctx.Configuration[$"{section}:{Constants.MonitoringUrl}"]
                    ?? throw new InvalidOperationException($"'{section}:{Constants.MonitoringUrl}' could not be read from configuration.");

                executeNowSchedule = ctx.Configuration[$"{section}:{Constants.ExecuteNowSchedule}"]
                    ?? throw new InvalidOperationException($"'{section}:{Constants.ExecuteNowSchedule}' could not be read from configuration.");
                
                ConfigureReporting<T>(ctx, services, jobHostName, monitoringUrl, executeNowSchedule);

            });

            return builder;
        }

        private static void ConfigureReporting<T>(HostBuilderContext ctx, IServiceCollection services, string jobHostName, string monitoringUrl, string executeNowSchedule) where T : class, IJobExecutionManager
        {
            services.Configure<JobReportOptions>(x =>
            {
                x.MonitoringUrl = monitoringUrl;
                x.ExecuteNowSchedule = executeNowSchedule;
                x.JobHostName = jobHostName;
            });

            services.TryAddSingleton<IJobExecutionManager, T>();

            ctx.Properties[Constants.UsesDashboard] = true;
            ctx.Properties[Constants.JobHostName] = jobHostName;

            services.AddQuartz(x =>
            {
                var matcher = GroupMatcher<JobKey>.GroupEquals(JobKey.DefaultGroup);
                x.AddJobListener<JobListener>(matcher);

                x.ScheduleJob<RegisterHost>(
                   x => x
                       .StartNow()
                       .WithPriority(int.MaxValue),
                   x => x
                       .WithIdentity(nameof(RegisterHost), nameof(RegisterHost))
                       .UsingJobData(new JobDataMap {
                            { Constants.JobHostName, jobHostName },
                            { Constants.MonitoringUrl, monitoringUrl },
                            { Constants.ExecuteNowSchedule, executeNowSchedule }
                       }));
            });
        }

        /// <summary>
        /// Extends the host builder to configure job monitoring with specified parameters.
        /// </summary>
        /// <param name="builder">The host builder to extend.</param>
        /// <param name="jobHostName">The name of the job host.</param>
        /// <param name="monitoringUrl">The URL for monitoring.</param>
        /// <param name="executeNowSchedule">Cron expression for polling interval for the execute now job(default is "0/15 * * * * ?").</param>
        /// <returns>The extended host builder.</returns>
        public static IHostBuilder UseJobReporting<T>(this IHostBuilder builder,
            string jobHostName,
            string monitoringUrl,
            string executeNowSchedule = "0/15 * * * * ?") where T : class, IJobExecutionManager
        {
            builder.ConfigureServices((ctx, services) =>
            {
                ConfigureReporting<T>(ctx, services, jobHostName, monitoringUrl, executeNowSchedule);
            });

            return builder;
        }

        /// <summary>
        /// Adds a hosted job to the host environment using a specified generic type 
        /// with its cron schedule retrieved from configuration.
        /// </summary>
        /// <param name="builder">The host builder to extend.</param>
        /// <param name="cronSchedule">The cron schedule for added Job.</param>
        /// <returns>The extended host builder.</returns>
        public static IHostBuilder AddHostedJob<TJob>(this IHostBuilder builder, string cronSchedule)
            where TJob : class, IJob
        {
            builder.ConfigureServices((ctx, services) =>
            {
                AddHostedJob<TJob>(ctx, services, cronSchedule);
            });

            return builder;
        }

        /// <summary>
        /// Adds a hosted job to the host environment based on whether a dashboard is being used or not.
        /// </summary>
        public static IHostBuilder AddHostedJob<TJob>(this IHostBuilder builder)
            where TJob : class, IJob
        {
            builder.ConfigureServices((ctx, services) =>
            {
                var schedule = ctx.Configuration[$"{Constants.DefaultConfigSection}:{typeof(TJob).Name}"]
                    ?? throw new InvalidOperationException($"'{Constants.DefaultConfigSection}:{typeof(TJob).Name}' could not be read from configuration.");
                if (schedule == "false")
                {
                    return;
                }

                AddHostedJob<TJob>(ctx, services, schedule);
            });

            return builder;
        }

        private static void AddHostedJob<TJob>(HostBuilderContext ctx, IServiceCollection services, string cronSchedule)
            where TJob : class, IJob
        {
            if (ctx.Properties.ContainsKey(Constants.UsesDashboard) && ctx.Properties[Constants.UsesDashboard] as bool? == true)
            {
                services.AddScoped<TJob>();
                services.AddScoped(x => new RegisterJob(typeof(TJob), cronSchedule));
            }
            else
            {
                services.AddQuartz(q =>
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
