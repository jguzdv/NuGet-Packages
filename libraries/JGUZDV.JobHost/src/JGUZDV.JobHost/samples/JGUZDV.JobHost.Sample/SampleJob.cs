using Quartz;

namespace JGUZDV.JobHost.Sample
{
    internal class SampleJob : IJob
    {
        public SampleJob()
        {
       
        }

        public ValueTask Execute(IJobExecutionContext context, CancellationToken cancellationToken)
        {
            Console.WriteLine("Job executed " + DateTime.Now);
            return ValueTask.CompletedTask;
        }
    }
}
