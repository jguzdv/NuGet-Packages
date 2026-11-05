using Quartz;

namespace JGUZDV.JobHost.Tests
{
    internal class TestJob : IJob
    {
        public TestJob(JobHostWrapper jobHostWrapper)
        {
            JobHostWrapper = jobHostWrapper;
        }

        public JobHostWrapper JobHostWrapper { get; set; }

        public ValueTask Execute(IJobExecutionContext context, CancellationToken cancellationToken)
        {
            JobHostWrapper.TestValue = true;
            return ValueTask.CompletedTask;
        }
    }

    internal class TestJob2 : IJob
    {
        public TestJob2(JobHostWrapper jobHostWrapper)
        {
            JobHostWrapper = jobHostWrapper;
        }

        public JobHostWrapper JobHostWrapper { get; set; }

        public ValueTask Execute(IJobExecutionContext context, CancellationToken cancellationToken)
        {
            JobHostWrapper.TestValue2 = true;
            return ValueTask.CompletedTask;
        }
    }

    internal class TestJob3 : IJob
    {
        public TestJob3(JobHostWrapper jobHostWrapper)
        {
            JobHostWrapper = jobHostWrapper;
        }

        public JobHostWrapper JobHostWrapper { get; set; }

        public ValueTask Execute(IJobExecutionContext context, CancellationToken cancellationToken)
        {
            JobHostWrapper.TestValue3 = true;
            return ValueTask.CompletedTask;
        }
    }

    internal class FailJob : IJob
    {
        public ValueTask Execute(IJobExecutionContext context, CancellationToken cancellationToken)
        {
            return ValueTask.FromException(new Exception());
        }
    }

    internal class JobHostWrapper
    {
        public bool TestValue { get; set; } = false;
        public bool TestValue2 { get; set; } = false;
        public bool TestValue3 { get; set; } = false;
       
    }

}
