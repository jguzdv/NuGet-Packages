using JGUZDV.JobHost.Database;

namespace JGUZDV.JobHost.Dashboard.Sample
{
    public static class SeedData
    {
        public static void AddSeedData(JobHostContext context)
        {
            var hosts = Enumerable
                .Range(0, 3)
                .Select(x => new Database.Entities.Host
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
                    new Database.Entities.Job
                    {
                        Host = host,
                        LastExecutedAt = DateTime.UtcNow,
                        LastResult = i % mod == 0 ? "success" : "error",
                        Name = $"Job-{i}",
                        Schedule = "* 0/15 * * *",
                        ShouldExecute = i % mod == 0,
                    })
                    .ToList();

                context.Jobs.AddRange(jobs);
            }


            context.SaveChanges();
        }
    }
}

