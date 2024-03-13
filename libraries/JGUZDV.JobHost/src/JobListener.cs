using JGUZDV.JobHost.Shared;

using Quartz;
using Quartz.Listener;

namespace JGUZDV.JobHost
{
    internal class JobListener : JobListenerSupport
    {
        private readonly IJobExecutionReporter _reporter;

        public JobListener(IJobExecutionReporter reporter)
        {
            _reporter = reporter;
        }

        public override string Name => nameof(JobListener);

        public override async Task JobWasExecuted(IJobExecutionContext context, JobExecutionException? jobException, CancellationToken cancellationToken = default)
        {
            try
            {

                await _reporter.ReportJobExecutionAsync(new()
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
