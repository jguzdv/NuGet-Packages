using System.Data;

using JGUZDV.JobHost.Dashboard.EntityFrameworkCore;
using JGUZDV.JobHost.Shared.Model;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace JGUZDV.JobHost.Tests
{

    public class JobHostTest : IDisposable
    {
        private string _connectionString = "Server=(LocalDb)\\MSSQLLocalDB;Database=JobHostDashboard_Test;Trusted_Connection=True;MultipleActiveResultSets=true;Encrypt=False";

        [Fact]
        public async Task RegisterAndRunJobsTest()
        {
            var builder = JobHost.CreateJobHostBuilder(Array.Empty<string>(),
                configureWindowsService => configureWindowsService.ServiceName = "Test",
                quartzHostedServiceOptions => quartzHostedServiceOptions.WaitForJobsToComplete = true);
            var testObject = new JobHostWrapper();

            builder.Services.AddSingleton(testObject);

            builder.AddHostedJob<TestJob>();
            builder.AddHostedJob<TestJob2>();
            var host = builder.Build();
            _ = host.RunAsync();

            await Task.Delay(TimeSpan.FromSeconds(3));
            await host.StopAsync();

            Assert.True(testObject.TestValue);
            Assert.True(testObject.TestValue2);
        }

        [Fact]
        public async Task RegisterAndRunEmptyChonScheduleTest()
        {
            var builder = JobHost.CreateJobHostBuilder(Array.Empty<string>(),
                configureWindowsService => configureWindowsService.ServiceName = "Test",
                quartzHostedServiceOptions => quartzHostedServiceOptions.WaitForJobsToComplete = true);
            var testObject = new JobHostWrapper();
            builder.Services.AddSingleton(testObject);

            builder.AddHostedJob<TestJob3>();

            var host = builder.Build();
            _ = host.RunAsync();

            await Task.Delay(TimeSpan.FromSeconds(2));
            await host.StopAsync();

            Assert.False(testObject.TestValue3);
        }


        private void InitDb()
        {
            var contextOptions = new DbContextOptionsBuilder<JobHostContext>()
                .UseSqlServer(_connectionString)
                .Options;

            using (var context = new JobHostContext(contextOptions))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
            }

        }

        [Fact]
        public async Task DashboardTest()
        {
            InitDb();

            var builder = JobHost.CreateJobHostBuilder(Array.Empty<string>(),
                    configureWindowsService => configureWindowsService.ServiceName = "DashboardTest",
                    quartzHostedServiceOptions => quartzHostedServiceOptions.WaitForJobsToComplete = true)
                .UseJobHostContextReporting(x =>
                {
                    x.JobHostName = "Test";
                    x.MonitoringUrl = "www.test.de";
                });

            var testObject = new JobHostWrapper();

            builder.Services.AddDbContextFactory<JobHostContext>(x => x.UseSqlServer(_connectionString));
            builder.Services.AddSingleton(testObject);

            builder.AddHostedJob<TestJob>();
            builder.AddHostedJob<TestJob2>();
            builder.AddHostedJob<FailJob>();

            var host = builder.Build();
            _ = host.RunAsync();
            await Task.Delay(TimeSpan.FromSeconds(2));

            using (var scope = host.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<JobHostContext>();

                var jobs = await dbContext.Jobs.ToListAsync();
                var hosts = await dbContext.Hosts.ToListAsync();

                Assert.Equal(3, jobs.Count);
                Assert.Single(hosts);

                Assert.Equal(Job.Success, jobs.Where(x => x.Name != nameof(FailJob)).ToList()[0].LastResult);
                Assert.Equal(Job.Success, jobs.Where(x => x.Name != nameof(FailJob)).ToList()[1].LastResult);
                Assert.Equal(Job.Error, jobs.Where(x => x.Name == nameof(FailJob)).ToList()[0].LastResult);
            }

            await host.StopAsync();

            Assert.True(testObject.TestValue);
            Assert.True(testObject.TestValue2);
        }

        [Fact]
        public async Task ExecuteNowTest()
        {
            InitDb();

            var builder = JobHost.CreateJobHostBuilder(Array.Empty<string>(),
                    configureWindowsService => configureWindowsService.ServiceName = "ExecuteNowTest",
                    quartzHostedServiceOptions => quartzHostedServiceOptions.WaitForJobsToComplete = true)
                .UseJobReporting<JobHostContextReporter>(x =>
                {
                    x.JobHostName = "Test";
                    x.MonitoringUrl = "www.test.de";
                    x.ExecuteNowSchedule = "* * * * * ?";
                });
            var testObject = new JobHostWrapper();

            builder.Services.AddDbContextFactory<JobHostContext>(x => x.UseSqlServer(_connectionString));
            builder.Services.AddSingleton(testObject);

            var now = DateTimeOffset.Now;
            builder.AddHostedJob<TestJob3>($"{(now.Second + 30) % 60} * * * * ?");

            var host = builder.Build();
            _ = host.RunAsync();

            await Task.Delay(TimeSpan.FromMilliseconds(1000));
            using (var scope = host.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<JobHostContext>();

                var job = await dbContext.Jobs.FirstOrDefaultAsync();
                job.ShouldExecuteAt = now;
                await dbContext.SaveChangesAsync();
            }

            await Task.Delay(TimeSpan.FromMilliseconds(1050));

            using (var scope = host.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<JobHostContext>();

                var job = await dbContext.Jobs.FirstOrDefaultAsync();

                Assert.True(job.ShouldExecuteAt <= job.LastExecutedAt);
                Assert.True(testObject.TestValue3);
            }

            await host.StopAsync();
        }

        public void Dispose()
        {
            var contextOptions = new DbContextOptionsBuilder<JobHostContext>()
                .UseSqlServer(_connectionString)
                .Options;

            using (var context = new JobHostContext(contextOptions))
            {
                context.Database.EnsureDeleted();
            }
        }
    }
}