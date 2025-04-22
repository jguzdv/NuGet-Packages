using JGUZDV.AspNetCore.Hosting;
using JGUZDV.AspNetCore.Hosting.Components;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

using ZDV.BlazorWebTemplate.Web;
using ZDV.BlazorWebTemplate.Web.Client;
using ZDV.BlazorWebTemplate.Web.Client.HttpClients;
using ZDV.BlazorWebTemplate.Web.Components;

var builder = JGUZDVHostApplicationBuilder.CreateWebHost(
    args, BlazorInteractivityModes.WebAssembly);

if (builder.Configuration["ApiBaseURl"] is string apiBaseUrl)
{
    builder.Services.AddHttpClient<WebApiClient>(client =>
    {
        client.BaseAddress = new Uri(apiBaseUrl, UriKind.Absolute);
    });

    builder.Services.AddHttpClient<CustomCookieAuthenticationEvents>(
        client =>
        {
            client.BaseAddress = new Uri(apiBaseUrl);
        }
    );

    builder.Services.Configure<CookieAuthenticationOptions>(CookieAuthenticationDefaults.AuthenticationScheme,
        opt =>
        {
            opt.EventsType = typeof(CustomCookieAuthenticationEvents);
            opt.AccessDeniedPath = "/status/access-denied";
        });
}

builder.Services.Configure<AuthorizationOptions>(authz => authz.AddDefaultPolicies());


var app = builder.BuildAndConfigureBlazor<App>(
    typeof(ZDV.BlazorWebTemplate.Web.Client._Imports).Assembly
);
await app.RunAsync();
