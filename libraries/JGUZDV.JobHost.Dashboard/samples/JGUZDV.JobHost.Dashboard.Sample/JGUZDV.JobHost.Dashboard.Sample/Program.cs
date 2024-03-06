using JGUZDV.JobHost.Dashboard.Extensions;
using JGUZDV.JobHost.Dashboard.Sample;
using JGUZDV.JobHost.Dashboard.Sample.Components;
using JGUZDV.JobHost.Dashboard.Services;
using JGUZDV.JobHost.Database;

using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddLocalization();

builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddDbContextFactory<JobHostContext>(
       x =>   x.UseSqlServer("Server=(LocalDb)\\MSSQLLocalDB;Database=JobHostDashboardSample;Trusted_Connection=True;MultipleActiveResultSets=true;Encrypt=False")
    );

builder.Services.AddSingleton<IDashboardService, DatabaseService>();
builder.Services.AddDashboard();
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<JobHostContext>();
    context.Database.EnsureCreated();
    if(!context.Hosts.Any())
        SeedData.AddSeedData(context);
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
