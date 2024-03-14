using Quartz;

namespace JGUZDV.JobHost.Sample
{
    internal class SampleJob : IJob
    {
        public SampleJob()
        {
       
        }

        public Task Execute(IJobExecutionContext context)
        {
            Console.WriteLine("Job executed " + DateTime.Now);
            return Task.CompletedTask;
        }
    }
}
