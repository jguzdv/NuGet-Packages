using JGUZDV.JobHost.Database;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace JGUZDV.JobHost
{
    internal class MetaJob<T> : IJob where T : IJob
    {
        private readonly JobHostContext _dbContext;
        private readonly T _job;

        public MetaJob(JobHostContext dbContext, T job)
        {
            _dbContext = dbContext;
            _job = job;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var host = (string)context.JobDetail.JobDataMap[Constants.JobHostName];

            var name = typeof(T).Name;
            var jobs = await _dbContext.Jobs.ToListAsync();
            var job = await _dbContext.Jobs.FirstAsync(x => x.Name == name && x.Host!.Name == host);

            try
            {
                await _job.Execute(context);
                job.LastResult = "success";
            }
            catch
            {
                job.LastResult = "error";
            }
            finally
            {
                // Only valid as long as there is no rescheduling
                var now = DateTimeOffset.Now;
                job.NextExecutionAt = new CronExpression(job.Schedule).GetNextValidTimeAfter(now);
                job.LastExecutedAt = now;

                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
