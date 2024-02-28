using JGUZDV.JobHost.Database;

using Microsoft.EntityFrameworkCore;

using Quartz;
using Quartz.Impl.Matchers;

namespace JGUZDV.JobHost
{
    internal class RegisterHost : IJob
    {
        private readonly IEnumerable<RegisterJob> _jobs;
        private readonly ISchedulerFactory _schedulerFactory;
        private readonly JobHostContext _dbContext;

        public RegisterHost(JobHostContext dbContext, IEnumerable<RegisterJob> jobs, ISchedulerFactory schedulerFactory)
        {
            _jobs = jobs;
            _dbContext = dbContext;
            _schedulerFactory = schedulerFactory;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var scheduler = await _schedulerFactory.GetScheduler();

            await scheduler.PauseJobs(GroupMatcher<JobKey>.GroupEquals(JobKey.DefaultGroup));
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
                    await item.Execute(host, scheduler);
                }
            }
            catch (Exception e)
            {
                //log
            }
            finally
            {
                await scheduler.ResumeJobs(GroupMatcher<JobKey>.GroupEquals(JobKey.DefaultGroup));
            }
        }
    }
}
