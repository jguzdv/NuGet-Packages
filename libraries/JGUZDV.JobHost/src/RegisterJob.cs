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
        private readonly Type _jobType;

        public RegisterJob(JobHostContext dbContext, Type jobType, string cronSchedule)
        {
            _jobName = jobType.Name;
            _cronSchedule = cronSchedule;
            _dbContext = dbContext;
            _jobType = jobType;
        }

        public async Task Execute(int hostId, IScheduler scheduler)
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

            var jobDetail = JobBuilder
                .Create(_jobType)
                .WithIdentity(new JobKey(_jobType.Name))
                .Build();

            var trigger = TriggerBuilder.Create()
                .ForJob(jobDetail)
                .WithCronSchedule(_cronSchedule)
                .Build();

            await scheduler.ScheduleJob(jobDetail,trigger);
        }
    }
}
