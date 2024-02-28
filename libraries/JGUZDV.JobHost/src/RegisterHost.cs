using JGUZDV.JobHost.Database;

using Microsoft.EntityFrameworkCore;

using Quartz;

namespace JGUZDV.JobHost
{
    internal class RegisterHost : IJob
    {
        private readonly IEnumerable<RegisterJob> _jobs;
        private readonly IScheduler _scheduler;
        private readonly JobHostContext _dbContext;

        public RegisterHost(JobHostContext dbContext, IEnumerable<RegisterJob> jobs, IScheduler scheduler)
        {
            _jobs = jobs;
            _dbContext = dbContext;
            _scheduler = scheduler;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            await _scheduler.PauseAll();
            try
            {
                var hostName = (string)context.JobDetail.JobDataMap["JobHostName"];
                var host = await _dbContext.Hosts.FirstOrDefaultAsync(x => x.Name == hostName);
                if (host == null)
                {
                    host = new Database.Entities.Host
                    {
                        MonitoringUrl = (string)context.JobDetail.JobDataMap["MonitoringUrl"],
                        Name = hostName
                    };

                    _dbContext.Hosts.Add(host);
                    await _dbContext.SaveChangesAsync();
                }

                foreach (var item in _jobs)
                {
                    await item.Execute(host.Id);
                }
            }
            finally
            {
                await _scheduler.ResumeAll();
            }
        }
    }
}
