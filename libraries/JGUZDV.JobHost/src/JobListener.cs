using JGUZDV.JobHost.Abstractions;

using Quartz;
using Quartz.Listener;

namespace JGUZDV.JobHost
{
    internal class JobListener : JobListenerSupport
    {
        private readonly IJobExecutionReporterFactory _dbContextFactory;

        public JobListener(IJobExecutionReporterFactory dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public override string Name => nameof(JobListener);

        public override async Task JobWasExecuted(IJobExecutionContext context, JobExecutionException? jobException, CancellationToken cancellationToken = default)
        {
            try
            {
                var reporter = await _dbContextFactory.CreateAsync();

                await reporter.ReportJobExecutionAsync(new()
                {
                    Failed = jobException != null,
                    FailMessage = GetFailMessage(jobException),
                    FireTimeUtc = context.FireTimeUtc,
                    Host = (string)context.JobDetail.JobDataMap[Constants.JobHostName],
                    Name = context.JobInstance.GetType().Name,
                    NextFireTimeUtc = context.NextFireTimeUtc,
                    RunTime = context.JobRunTime
                });
            }
            catch
            {

            }
        }

        private string? GetFailMessage(Exception? exception)
        {
            if (exception == null)
            {
                return null;
            }

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
