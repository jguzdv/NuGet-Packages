// See https://aka.ms/new-console-template for more information
using JGUZDV.JobHost;
using JGUZDV.JobHost.Dashboard.EntityFrameworkCore;
using JGUZDV.JobHost.Sample;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = JobHost.CreateJobHostBuilder(args, configureWindowsService => configureWindowsService.ServiceName = "Sample",
                quartzHostedServiceOptions => quartzHostedServiceOptions.WaitForJobsToComplete = true);

builder.Services.AddDbContextFactory<JobHostContext>(x => x.UseSqlServer("Server=(LocalDb)\\MSSQLLocalDB;Database=JobHostDashboardSample;Trusted_Connection=True;MultipleActiveResultSets=true;Encrypt=False"));

builder.UseJobReporting<JobHostContextReporter>();
builder.AddHostedJob<SampleJob>();

var host = builder.Build();
using (var scope = host.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<JobHostContext>();
    context.Database.EnsureDeleted();
    context.Database.EnsureCreated();
}

host.Run();
