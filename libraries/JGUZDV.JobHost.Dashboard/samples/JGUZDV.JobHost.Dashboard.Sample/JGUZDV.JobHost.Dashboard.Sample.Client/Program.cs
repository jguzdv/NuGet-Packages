using JGUZDV.JobHost.Dashboard.Services;

using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddTransient<IDashboardService, ApiClient>(x =>
{
    var baseUrl = new Uri(new Uri(builder.HostEnvironment.BaseAddress), "api/");
    var client = new HttpClient();
    client.BaseAddress = baseUrl;
    return new ApiClient(client);
});

await builder.Build().RunAsync();
