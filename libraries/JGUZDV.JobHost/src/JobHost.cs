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

                                ctx.Properties["UsesDashboard"] = true;
                            })
                           .UseJGUZDVLogging()
                           .UseWindowsService(configureWindowsService);


            return builder;
        }
              
        public static IHostBuilder AddHostedJob<TJob>(this IHostBuilder builder, string cronSchedule)
            where TJob : class, IJob
        {
            builder.ConfigureServices((ctx, services) =>
            {
                AddHostedJob<TJob>(services, cronSchedule);
            });

            return builder;
        }
        
        public static IHostBuilder AddHostedJob<TJob>(this IHostBuilder builder)
            where TJob : class, IJob
        {
            builder.ConfigureServices((ctx, services) => {
                var schedule = ctx.Configuration[$"{DefaultConfigSection}:{typeof(TJob).Name}"] 
                    ?? throw new InvalidOperationException($"'{DefaultConfigSection}:{typeof(TJob).Name}' could not be read from configuration.");
                if(schedule == "false")
                {
                    return;
                }

                AddHostedJob<TJob>(services, schedule);
            });

            return builder;
        }

        private static void AddHostedJob<TJob>(IServiceCollection services, string cronSchedule) 
            where TJob : class, IJob
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
