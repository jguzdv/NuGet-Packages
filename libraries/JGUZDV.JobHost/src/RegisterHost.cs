using JGUZDV.JobHost.Database;
using JGUZDV.JobHost.Database.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace JGUZDV.JobHost
{
    internal class RegisterHost : IHostedService
    {
        private IEnumerable<RegisterJob> _jobs;
        private string _jobHostName;
        private readonly JobHostContext _dbContext;

        public RegisterHost(JobHostContext dbContext, IEnumerable<RegisterJob> jobs, string jobHostName)
        {
            _jobs = jobs;
            _jobHostName = jobHostName;
            _dbContext = dbContext;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var host = await _dbContext.Hosts.FirstOrDefaultAsync(x => x.Name == _jobHostName);
            if (host == null)
            {
                host = new Database.Entities.Host
                {
                    MonitoringUrl = _jobHostName,
                    Name = _jobHostName
                };

                _dbContext.Hosts.Add(host);
                await _dbContext.SaveChangesAsync();
            }

            foreach(var item in _jobs)
            {
                await item.Execute(host.Id);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
