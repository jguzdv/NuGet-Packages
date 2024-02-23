using JGUZDV.JobHost.Dashboard.Extensions;
using JGUZDV.JobHost.Dashboard.Sample.Components;
using JGUZDV.JobHost.Dashboard.Services;
using JGUZDV.JobHost.Database;

using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddDbContext<JobHostContext>(
        x => x.UseInMemoryDatabase("MemoryDatabase")
    );

builder.Services.AddScoped<IDashboardService, DatabaseService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<JobHostContext>();

    var host = new JGUZDV.JobHost.Database.Entities.Host
    {
        MonitoringUrl = "raps.test.url",
        Name = "Raps-ETL"
    };
    context.Hosts.Add(host);

    var job = new JGUZDV.JobHost.Database.Entities.Job
    {
        Host = host,
        LastExecutedAt = DateTime.UtcNow,
        LastResult = "success",
        LastResultMessage = "success",
        Name = "SyncBooking",
        Schedule = "* 0/15 * * *",
        ShouldExecute = false,
    };

    context.Jobs.Add(job);
    context.SaveChanges();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseAntiforgery();

app.MapJobHostDashboardApi("api");

app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(JGUZDV.JobHost.Dashboard.Sample.Client._Imports).Assembly);

app.Run();
