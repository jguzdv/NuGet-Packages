using JGUZDV.Blazor.Hosting;

using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

using ZDV.BlazorWebTemplate.Web.Client;

var builder = JGUZDVWebAssemblyApplicationBuilder.CreateDefault(args);

if (builder.Environment.IsDevelopment())
{
    builder.Logging.SetMinimumLevel(LogLevel.Debug);
}


builder.Services.AddAuthorizationCore(opt => opt.AddDefaultPolicies());

var app = await builder.BuildAsync();
await app.RunAsync();
