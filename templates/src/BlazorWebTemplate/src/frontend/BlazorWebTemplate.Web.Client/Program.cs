using JGUZDV.Blazor.Hosting;

using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

using ZDV.BlazorWebTemplate.Web.Client;
using ZDV.BlazorWebTemplate.Web.Client.HttpClients;

var builder = JGUZDVWebAssemblyApplicationBuilder.CreateDefault(args);

if (builder.Environment.IsDevelopment())
{
    builder.Logging.SetMinimumLevel(LogLevel.Debug);
}

builder.Services.AddHttpClient<WebApiClient>(client =>
{
    client.BaseAddress = new Uri(new Uri(builder.Environment.BaseAddress), "api");
});


builder.Services.AddAuthorizationCore(opt => opt.AddDefaultPolicies());

var app = await builder.BuildAsync();
await app.RunAsync();
