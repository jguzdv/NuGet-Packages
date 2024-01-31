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

        public Task Execute(IJobExecutionContext context)
        {
            JobHostWrapper.TestValue = true;
            return Task.CompletedTask;
        }
    }

    internal class TestJob2 : IJob
    {
        public TestJob2(JobHostWrapper jobHostWrapper)
        {
            JobHostWrapper = jobHostWrapper;
        }

        public JobHostWrapper JobHostWrapper { get; set; }

        public Task Execute(IJobExecutionContext context)
        {
            JobHostWrapper.TestValue2 = true;
            return Task.CompletedTask;
        }
    }

    internal class JobHostWrapper
    {
        public bool TestValue { get; set; } = false;
        public bool TestValue2 { get; set; } = false;
    }

}
