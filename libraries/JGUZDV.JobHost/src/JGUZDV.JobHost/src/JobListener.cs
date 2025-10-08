using JGUZDV.JobHost.Shared;

using Microsoft.Extensions.Logging;

using Quartz;
using Quartz.Listener;

namespace JGUZDV.JobHost
{
    internal class JobListener : JobListenerSupport
    {
        private readonly IJobExecutionManager _reporter;
        private readonly ILogger<JobListener> _logger;

        public JobListener(IJobExecutionManager reporter, ILogger<JobListener> logger)
        {
            _reporter = reporter;
            _logger = logger;
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
            catch (Exception e)
            {
                _logger.LogError(e, "Error reporting job execution");
            }
        }

        private string? GetFailMessage(Exception? exception)
        {
            if (exception == null)
            {
                return null;
            }

            while (exception is SchedulerException && exception.InnerException != null)
            {
                exception = exception.InnerException;
            }

            return exception.Message;
        }
    }
}
