using JGUZDV.JobHost.Database;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Quartz;

namespace JGUZDV.JobHost
{
    public static class JobHost
    {

        public const string DefaultConfigSection = "HostedJobs";

        public static IHostBuilder CreateJobHostBuilder(string[] args, Action<WindowsServiceLifetimeOptions> configureWindowsService, Action<QuartzHostedServiceOptions> configureQuartz)
        {
            var builder = Host.CreateDefaultBuilder(args)
                            .ConfigureServices((ctx, services) =>
                            {
                                services.AddQuartzHostedService(configureQuartz);
                            })
                           .UseJGUZDVLogging()
                           .UseWindowsService(configureWindowsService);

            return builder;
        }

        public static IHostBuilder UseDashboard(this IHostBuilder builder,
            string jobHostName,
            string monitoringUrl,
            Action<DbContextOptionsBuilder, IConfiguration> configureDbContext)
        {
            builder.ConfigureServices((ctx, services) =>
            {
                services.AddDbContext<JobHostContext>(x => configureDbContext(x, ctx.Configuration));
                ctx.Properties["UsesDashboard"] = true;
                ctx.Properties["JobHostName"] = jobHostName;

                services.AddQuartz(x => x.ScheduleJob<RegisterHost>(x => x
                    .StartNow()
                    .WithPriority(int.MaxValue), 
                    x => x.UsingJobData(new JobDataMap { { "JobHostName", jobHostName }, { "MonitoringUrl", monitoringUrl} })));
            });

            return builder;
        }

        public static IHostBuilder AddHostedJob<TJob>(this IHostBuilder builder, string cronSchedule)
            where TJob : class, IJob
        {
            builder.ConfigureServices((ctx, services) =>
            {
                AddHostedJob<TJob>(ctx, services, cronSchedule);
            });

            return builder;
        }

        public static IHostBuilder AddHostedJob<TJob>(this IHostBuilder builder)
            where TJob : class, IJob
        {
            builder.ConfigureServices((ctx, services) =>
            {
                var schedule = ctx.Configuration[$"{DefaultConfigSection}:{typeof(TJob).Name}"]
                    ?? throw new InvalidOperationException($"'{DefaultConfigSection}:{typeof(TJob).Name}' could not be read from configuration.");
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
            if (ctx.Properties.ContainsKey("UsesDashboard") && ctx.Properties["UsesDashboard"] as bool? == true)
            {
                services.AddQuartz(q =>
                {
                    var jobKey = new JobKey(typeof(TJob).Name);
                    q.AddJob<TheJob<TJob>>(x => x.UsingJobData("JobHostName", (string)ctx.Properties["JobHostName"]));
                    q.AddTrigger(opts => opts
                        .ForJob(jobKey)
                        .StartAt(DateTimeOffset.Now.AddMilliseconds(50)) // TODO: check if necessary and/or better solution available
                        .WithCronSchedule(cronSchedule));

                    services.AddScoped(x => new RegisterJob(x.GetRequiredService<JobHostContext>(), jobKey.Name, cronSchedule));
                });
            }
            else
            {
                services.AddQuartz(q =>
                {
                    var jobKey = new JobKey(typeof(TJob).Name);
                    q.AddJob<TJob>(jobKey);
                    q.AddTrigger(opts => opts
                        .ForJob(jobKey)
                        .WithCronSchedule(cronSchedule));
                });
            }
        }
    }
}
