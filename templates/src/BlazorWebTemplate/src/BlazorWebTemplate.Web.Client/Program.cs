using JGUZDV.Blazor.Hosting;

using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

using ZDV.JGUDir.Web.Client;
using ZDV.JGUDir.Web.SelfService.Client;
using ZDV.JGUDir.Web.SelfService.Client.HttpClients;
using ZDV.JGUDir.Web.SelfService.Client.Pages;
using ZDV.JGUDir.Web.SelfService.Client.Services;

var builder = JGUZDVWebAssemblyApplicationBuilder.CreateDefault(args);

if (builder.Environment.IsDevelopment())
{
    builder.Logging.SetMinimumLevel(LogLevel.Debug);
}


builder.Services.AddAuthorizationCore(opt => opt.AddDefaultPolicies());

var app = await builder.BuildAsync();
await app.RunAsync();
