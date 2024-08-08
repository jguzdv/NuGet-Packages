using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Quartz;
using Quartz.Impl.Matchers;

namespace JGUZDV.JobHost
{
    internal class JobHostBuilder : IHostBuilder
    {
        private readonly IHostBuilder _hostBuilder;

        public JobHostBuilder(IHostBuilder hostBuilder)
        {
            _hostBuilder = hostBuilder;
        }

        public IDictionary<object, object> Properties => _hostBuilder.Properties;

        public IHost Build()
        {
            var debug = false;
            _hostBuilder.ConfigureServices((ctx, services) =>
            {
                if (ctx.HostingEnvironment.IsDevelopment())
                {
                    while (true)
                    {
                        Console.WriteLine("Do you want to debug? (y/n)");
                        var input = Console.ReadLine();

                        if ("n".Equals(input, StringComparison.OrdinalIgnoreCase))
                            break;

                        if (!"y".Equals(input, StringComparison.OrdinalIgnoreCase))
                            continue;

                        debug = true;
                        break;
                    }
                }
            });

            var host = _hostBuilder.Build();
            if (debug)
            {
                using var scope = host.Services.CreateScope();
                var schedulerFactors = scope.ServiceProvider.GetRequiredService<ISchedulerFactory>();
                var scheduler = schedulerFactors.GetScheduler().Result;

                scheduler.PauseJobs(GroupMatcher<JobKey>.AnyGroup());
                var keys = scheduler.GetJobKeys(GroupMatcher<JobKey>.AnyGroup()).Result;

                foreach(var key in keys)
                {
                    Console.WriteLine();
                }
            }

            return host;
        }

        public IHostBuilder ConfigureAppConfiguration(Action<HostBuilderContext, IConfigurationBuilder> configureDelegate)
            => _hostBuilder.ConfigureAppConfiguration(configureDelegate);

        public IHostBuilder ConfigureContainer<TContainerBuilder>(Action<HostBuilderContext, TContainerBuilder> configureDelegate)
            => _hostBuilder.ConfigureContainer<TContainerBuilder>(configureDelegate);

        public IHostBuilder ConfigureHostConfiguration(Action<IConfigurationBuilder> configureDelegate)
            => _hostBuilder.ConfigureHostConfiguration(configureDelegate);

        public IHostBuilder ConfigureServices(Action<HostBuilderContext, IServiceCollection> configureDelegate)
            => ConfigureServices(configureDelegate);

        public IHostBuilder UseServiceProviderFactory<TContainerBuilder>(IServiceProviderFactory<TContainerBuilder> factory) where TContainerBuilder : notnull
            => _hostBuilder.UseServiceProviderFactory(factory);

        public IHostBuilder UseServiceProviderFactory<TContainerBuilder>(Func<HostBuilderContext, IServiceProviderFactory<TContainerBuilder>> factory) where TContainerBuilder : notnull
            => _hostBuilder.UseServiceProviderFactory<TContainerBuilder>(factory);
    }
}
