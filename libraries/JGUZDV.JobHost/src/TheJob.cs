using System.Xml.Linq;

using JGUZDV.JobHost.Database;

using Microsoft.EntityFrameworkCore;

using Quartz;

namespace JGUZDV.JobHost
{
    internal class TheJob<T> : IJob where T : IJob
    {
        private readonly JobHostContext _dbContext;
        private readonly T _job;

        public TheJob(JobHostContext dbContext, T job)
        {
            _dbContext = dbContext;
            _job = job;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var host = (string)context.JobDetail.JobDataMap["JobHostName"];

            var name = typeof(T).Name;
            var job = await _dbContext.Jobs.FirstAsync(x => x.Name == name && x.Host!.Name == host);

            var now = DateTimeOffset.Now;
            var nextExecution = new CronExpression(job.Schedule).GetNextValidTimeAfter(now);

            try
            {
                await _job.Execute(context);

                job.NextExecutionAt = nextExecution;

                job.LastResult = "success";
                job.LastExecutedAt = now;
            }
            catch
            {
                //TODO job.NextExecutionAt - check retry policy and set based on that?

                job.LastResult = "success";
                job.LastExecutedAt = now;
            }
            finally
            {
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
