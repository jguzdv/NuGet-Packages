using JGUZDV.JobHost.Dashboard.Services;

using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services.AddTransient<IDashboardService, ApiClient>();

await builder.Build().RunAsync();
