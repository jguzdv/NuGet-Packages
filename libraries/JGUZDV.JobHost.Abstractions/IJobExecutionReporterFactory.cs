
using Microsoft.Extensions.DependencyInjection;

namespace JGUZDV.JobHost.Abstractions
{
    public interface IJobExecutionReporterFactory
    {
        public IJobExecutionReporter Create();

        public Task<IJobExecutionReporter> CreateAsync();
    }

    public class JobExecutionReporterFactory : IJobExecutionReporterFactory
    {
        private IServiceProvider _services;

        public JobExecutionReporterFactory(IServiceProvider services)
        {
            _services = services;
        }

        public IJobExecutionReporter Create()
        {
             return _services.GetRequiredService<IJobExecutionReporter>();
        }

        public Task<IJobExecutionReporter> CreateAsync()
        {
            return Task.FromResult(_services.GetRequiredService<IJobExecutionReporter>());
        }
    }
}
