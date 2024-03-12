using System.Data;

using JGUZDV.JobHost.Abstractions;
using JGUZDV.JobHost.Database;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace JGUZDV.JobHost.Tests
{

    public class JobHostTest
    {
        [Fact]
        public async Task RegisterAndRunJobsTest()
        {
            var builder = JobHost.CreateJobHostBuilder(Array.Empty<string>(),
                configureWindowsService => configureWindowsService.ServiceName = "Test",
                quartzHostedServiceOptions => quartzHostedServiceOptions.WaitForJobsToComplete = true);
            var testObject = new JobHostWrapper();

            builder.ConfigureServices((ctx, services) =>
            {
                services.AddSingleton(testObject);
            });

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
            builder.ConfigureServices((ctx, services) =>
            {
                services.AddSingleton(testObject);
            });

            builder.AddHostedJob<TestJob3>();

            var host = builder.Build();
            _ = host.RunAsync();

            await Task.Delay(TimeSpan.FromSeconds(2));
            await host.StopAsync();

            Assert.False(testObject.TestValue3);
        }


        [Fact]
        public async Task DashboardTest()
        {
            var builder = JobHost.CreateJobHostBuilder(Array.Empty<string>(),
                    configureWindowsService => configureWindowsService.ServiceName = "DashboardTest",
                    quartzHostedServiceOptions => quartzHostedServiceOptions.WaitForJobsToComplete = true)
                .ConfigureServices(services =>
                {
                    services.AddDbContextFactory<JobHostContext>(x => x.UseInMemoryDatabase("DashboardTest"));
                    services.AddSingleton<IJobExecutionReporterFactory, TestFactory>();
                })
                .UseJobReporting<JobHostContext>("Test", "www.test.de");

            var testObject = new JobHostWrapper();

            builder.ConfigureServices((ctx, services) =>
            {
                services.AddSingleton(testObject);
            });

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

                //Assert.Equal(3, jobs.Count);
                Assert.Single(hosts);

                Assert.Equal("success", jobs.Where(x => x.Name != nameof(FailJob)).ToList()[0].LastResult);
                Assert.Equal("success", jobs.Where(x => x.Name != nameof(FailJob)).ToList()[1].LastResult);
                Assert.Equal("error", jobs.Where(x => x.Name == nameof(FailJob)).ToList()[0].LastResult);
            }

            await host.StopAsync();

            Assert.True(testObject.TestValue);
            Assert.True(testObject.TestValue2);
        }

        [Fact]
        public async Task ExecuteNowTest()
        {
            var builder = JobHost.CreateJobHostBuilder(Array.Empty<string>(),
                    configureWindowsService => configureWindowsService.ServiceName = "ExecuteNowTest",
                    quartzHostedServiceOptions => quartzHostedServiceOptions.WaitForJobsToComplete = true)
                .ConfigureServices(services => services.AddDbContext<JobHostContext>(x => x.UseInMemoryDatabase("ExecuteNowTest")))
                .UseJobReporting<JobHostContext>("Test", "www.test.de",
                    "* * * * * ?");
            var testObject = new JobHostWrapper();

            builder.ConfigureServices((ctx, services) =>
            {
                services.AddSingleton(testObject);
            });

            var now = DateTimeOffset.Now;
            builder.AddHostedJob<TestJob3>($"{(now.Second + 30) % 60} * * * * ?");

            var host = builder.Build();
            _ = host.RunAsync();

            await Task.Delay(TimeSpan.FromMilliseconds(1000));
            using (var scope = host.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<JobHostContext>();

                var job = await dbContext.Jobs.FirstOrDefaultAsync();
                job.ShouldExecute = true;
                await dbContext.SaveChangesAsync();
            }

            await Task.Delay(TimeSpan.FromMilliseconds(1050));

            using (var scope = host.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<JobHostContext>();

                var job = await dbContext.Jobs.FirstOrDefaultAsync();

                Assert.False(job.ShouldExecute);
                Assert.True(testObject.TestValue3);
            }

            await host.StopAsync();
        }
    }
}