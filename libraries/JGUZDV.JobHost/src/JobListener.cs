using JGUZDV.JobHost.Database;

using Microsoft.EntityFrameworkCore;

using Quartz;
using Quartz.Listener;

namespace JGUZDV.JobHost
{
    internal class JobListener : JobListenerSupport
    {
        private readonly IDbContextFactory<JobHostContext> _dbContextFactory;

        public JobListener(IDbContextFactory<JobHostContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public override string Name => nameof(JobListener);

        public override async Task JobWasExecuted(IJobExecutionContext context, JobExecutionException? jobException, CancellationToken cancellationToken = default)
        {
            try
            {
                var dbContext = await _dbContextFactory.CreateDbContextAsync();
                var name = context.JobInstance.GetType().Name;
                var host = (string)context.JobDetail.JobDataMap[Constants.JobHostName];

                var job = await dbContext.Jobs.FirstAsync(x => x.Name == name && x.Host!.Name == host);

                if (jobException == null)
                {
                    job.LastResult = "success";
                }
                else
                {
                    job.LastResult = "error";
                    job.FailMessage = GetFailMessage(jobException);
                }

                job.RunTime = context.JobRunTime;
                job.NextExecutionAt = context.NextFireTimeUtc;
                job.LastExecutedAt = context.FireTimeUtc;
                
                await dbContext.SaveChangesAsync();
            }
            catch
            {

            }
        }

        private string GetFailMessage(Exception exception)
        {
            while(true)
            {
                if (exception is not SchedulerException || exception.InnerException == null)
                {
                    return exception.Message;
                }

                exception = exception.InnerException;
            }
        }
    }
}
