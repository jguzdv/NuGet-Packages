using JGUZDV.JobHost.Dashboard;
using JGUZDV.JobHost.Dashboard.Extensions;
using JGUZDV.JobHost.Dashboard.Shared;
using JGUZDV.JobHost.Shared;

using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services.AddDashboard();
builder.Services.AddLocalization();
builder.Services.AddTransient<IDashboardService, ApiClient>(x =>
{
    var baseUrl = new Uri(new Uri(builder.HostEnvironment.BaseAddress), "api/");
    var client = new HttpClient();
    client.BaseAddress = baseUrl;
    return new ApiClient(client);
});

await builder.Build().RunAsync();
