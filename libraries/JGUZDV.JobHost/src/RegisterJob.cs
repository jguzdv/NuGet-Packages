using Quartz;

namespace JGUZDV.JobHost
{
    internal class RegisterJob
    {
        private readonly string _jobName;
        private readonly string _cronSchedule;
        private readonly Type _jobType;

        public string JobName => _jobName;
        public string CronSchedule => _cronSchedule;


        public RegisterJob(Type jobType, string cronSchedule)
        {
            _jobName = jobType.Name;
            _cronSchedule = cronSchedule;
            _jobType = jobType;
        }

        public async Task Execute(string hostName, IScheduler scheduler)
        {
            var jobDetail = JobBuilder
                .Create(_jobType)
                .WithIdentity(new JobKey(_jobType.Name))
                .UsingJobData(Constants.JobHostName, hostName)
                .DisallowConcurrentExecution()
                .Build();

            var trigger = TriggerBuilder.Create()
                .ForJob(jobDetail)
                .WithCronSchedule(_cronSchedule)
                .Build();

            await scheduler.ScheduleJob(jobDetail, trigger);
        }
    }
}
