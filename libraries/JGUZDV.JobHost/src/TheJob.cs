using JGUZDV.JobHost.Database;

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
            try
            {
                await _job.Execute(context);
                //write success
            }
            catch
            {
                //write fail
            }
        }
    }
}
