using JGUZDV.JobHost.Dashboard.EntityFrameworkCore;
using JGUZDV.JobHost.Shared.Model;

namespace JGUZDV.JobHost.Dashboard.Sample
{
    public static class SeedData
    {
        public static void AddSeedData(JobHostContext context)
        {
            var hosts = Enumerable
                .Range(0, 3)
                .Select(x => new EntityFrameworkCore.Entities.Host
                {
                    MonitoringUrl = $"www.url-{x}.com",
                    Name = $"Host-{x}"
                })
                .ToList();

            context.Hosts.AddRange(hosts);

            foreach (var host in hosts)
            {
                var x = new Random();

                var mod = x.Next(2, 3);
                var jobs = Enumerable
                    .Range(0, x.Next(2, 5))
                    .Select(i =>
                    new EntityFrameworkCore.Entities.Job
                    {
                        Host = host,
                        LastExecutedAt = DateTime.UtcNow,
                        LastResult = i % mod == 0 ? Job.Success : Job.Error,
                        FailMessage = i % mod == 0 ? null : "Critical error during execution - uh OH",
                        Name = $"Job-{i}",
                        Schedule = "* 0/15 * * *",
                        ShouldExecuteAt = DateTimeOffset.Now,
                    })
                    .ToList();

                context.Jobs.AddRange(jobs);
            }


            context.SaveChanges();
        }
    }
}

