using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace JGUZDV.JobHost.Tests
{

    public class JobHostTest
    {
        [Fact]
        public async Task RegisterAndRunJobsTest()
        {
            var builder = JobHost.CreateJobHostBuilder(Array.Empty<string>(), (wslo) => wslo.ServiceName = "Test", (cq) => cq.WaitForJobsToComplete = true);
            var testObject = new JobHostWrapper();

            builder.ConfigureServices((ctx, services) => {
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