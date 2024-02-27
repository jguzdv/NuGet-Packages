using JGUZDV.JobHost.Database;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace JGUZDV.JobHost
{
    internal class RegisterJob
    {
        private readonly string _jobName;
        private readonly string _cronSchedule;
        private readonly JobHostContext _dbContext;

        public RegisterJob(JobHostContext dbContext, string jobName, string cronSchedule)
        {
            _jobName = jobName;
            _cronSchedule = cronSchedule;
            _dbContext = dbContext;
        }

        public async Task Execute(int hostId)
        {
            var job = await _dbContext.Jobs.FirstOrDefaultAsync(x => x.HostId == hostId && x.Name == _jobName);
            if (job != null)
                return;

            var cron = new CronExpression(_cronSchedule);
            var nextExecutionAt = cron.GetNextValidTimeAfter(DateTimeOffset.Now);

            job = new Database.Entities.Job
            {
                LastExecutedAt = DateTimeOffset.MinValue,
                LastResult = "",
                NextExecutionAt = nextExecutionAt,
                LastResultMessage = "",
                Schedule = _cronSchedule,
                Name = _jobName,
                HostId = hostId,
            };

            await _dbContext.SaveChangesAsync();
        }
    }
}
