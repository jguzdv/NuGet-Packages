using JGUZDV.JobHost.Database;
using JGUZDV.JobHost.Database.Entities;

using Microsoft.EntityFrameworkCore;

using Quartz;

namespace JGUZDV.JobHost
{
    internal class RegisterJob
    {
        private readonly string _jobName;
        private readonly string _cronSchedule;
        private readonly Type _jobType;

        public string JobName => _jobName;

        public RegisterJob(Type jobType, string cronSchedule)
        {
            _jobName = jobType.Name;
            _cronSchedule = cronSchedule;
            _jobType = jobType;
        }

        public async Task Execute(Host host, IScheduler scheduler, JobHostContext dbContext)
        {
            var job = await dbContext.Jobs.FirstOrDefaultAsync(x => x.HostId == host.Id && x.Name == _jobName);
            
            if (job == null)
            {
                job = new Database.Entities.Job
                {
                    LastExecutedAt = DateTimeOffset.MinValue,
                    LastResult = "",
                    Schedule = _cronSchedule,
                    Name = _jobName,
                    HostId = host.Id,
                };

                dbContext.Jobs.Add(job);
            }

            var cron = new CronExpression(_cronSchedule);
            job.NextExecutionAt = cron.GetNextValidTimeAfter(DateTimeOffset.Now);

            await dbContext.SaveChangesAsync();

            var jobDetail = JobBuilder
                .Create(typeof(MetaJob<>).MakeGenericType(_jobType))
                .WithIdentity(new JobKey(_jobType.Name))
                .UsingJobData(Constants.JobHostName, host.Name)
                .Build();

            var trigger = TriggerBuilder.Create()
                .ForJob(jobDetail)
                .WithCronSchedule(_cronSchedule)
                .Build();

            await scheduler.ScheduleJob(jobDetail, trigger);
        }
    }
}
