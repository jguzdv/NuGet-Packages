using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Quartz;

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

            await Task.Delay(TimeSpan.FromSeconds(2));
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
                    configureWindowsService => configureWindowsService.ServiceName = "Test",
                    quartzHostedServiceOptions => quartzHostedServiceOptions.WaitForJobsToComplete = true)
                .UseDashboard("Test","www.test.de",
                    (x,y) => x.UseInMemoryDatabase("testdb"));
            var testObject = new JobHostWrapper();

            builder.ConfigureServices((ctx, services) =>
            {
                services.AddSingleton(testObject);
            });

            builder.AddHostedJob<TestJob>();
            builder.AddHostedJob<TestJob2>();
            var host = builder.Build();
            _ = host.RunAsync();

            await Task.Delay(TimeSpan.FromSeconds(2));
            await host.StopAsync();

            Assert.True(testObject.TestValue);
            Assert.True(testObject.TestValue2);
        }
    }
}