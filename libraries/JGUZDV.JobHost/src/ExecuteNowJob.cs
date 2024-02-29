using JGUZDV.JobHost.Database;

using Microsoft.EntityFrameworkCore;

using Quartz;

namespace JGUZDV.JobHost
{
    internal class ExecuteNowJob : IJob
    {
        private readonly JobHostContext _dbContext;
        private readonly ISchedulerFactory _schedulerFactory;

        public ExecuteNowJob(JobHostContext dbContext, ISchedulerFactory schedulerFactory)
        {
            _dbContext = dbContext;
            _schedulerFactory = schedulerFactory;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var hostName = (string)context.JobDetail.JobDataMap["JobHostName"];
            var jobs = await _dbContext.Jobs.Where(x => x.Host.Name == hostName && x.ShouldExecute == true).ToListAsync();
            var scheduler = await _schedulerFactory.GetScheduler();

            foreach (var job in jobs)
            {
                job.ShouldExecute = false;
                await _dbContext.SaveChangesAsync();

                var jobKey = new JobKey(job.Name);
                await scheduler.TriggerJob(jobKey);
            }
        }
    }
}
