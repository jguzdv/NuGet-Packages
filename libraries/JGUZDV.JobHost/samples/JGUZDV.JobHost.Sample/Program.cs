// See https://aka.ms/new-console-template for more information
using JGUZDV.JobHost;
using JGUZDV.JobHost.Database;
using JGUZDV.JobHost.Sample;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

Console.WriteLine("Hello, World!");

var builder = JobHost.CreateJobHostBuilder(args, configureWindowsService => configureWindowsService.ServiceName = "Sample",
                quartzHostedServiceOptions => quartzHostedServiceOptions.WaitForJobsToComplete = true);


builder.UseDashboard((x,y) =>
{
    x.UseSqlServer("Server=(LocalDb)\\MSSQLLocalDB;Database=JobHostDashboardSample;Trusted_Connection=True;MultipleActiveResultSets=true;Encrypt=False");
});
builder.AddHostedJob<SampleJob>();

var host = builder.Build();
using(var scope =  host.Services.CreateScope()){
    var context = scope.ServiceProvider.GetRequiredService<JobHostContext>();
    context.Database.EnsureCreated();
}

host.Run();
