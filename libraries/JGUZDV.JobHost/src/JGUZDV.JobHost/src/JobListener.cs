using JGUZDV.JobHost.Shared;

using Microsoft.Extensions.Logging;

using Quartz;
using Quartz.Extensibility;

namespace JGUZDV.JobHost
{
    internal class JobListener : IJobListener
    {
        private readonly IJobExecutionManager _reporter;
        private readonly ILogger<JobListener> _logger;

        public JobListener(IJobExecutionManager reporter, ILogger<JobListener> logger)
        {
            _reporter = reporter;
            _logger = logger;
        }

        public string Name => nameof(JobListener);

        public async ValueTask JobWasExecuted(IJobExecutionContext context, JobExecutionException? jobException, CancellationToken cancellationToken = default)
        {
            //TODO: make configurable and/or use a background queue for reporting
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(TimeSpan.FromSeconds(10));

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
                }, cts.Token);
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

        public ValueTask JobProgressChanged(IJobExecutionContext context, FireInstanceProgress progress, CancellationToken cancellationToken = default(CancellationToken))
        {
            _logger.LogInformation(progress.Message);
            return default(ValueTask);
        }

        public ValueTask JobToBeExecuted(IJobExecutionContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
            _logger.LogInformation($"Job {context.JobDetail.Key} is about to be executed.");
            return default(ValueTask);
        }
    }
}
