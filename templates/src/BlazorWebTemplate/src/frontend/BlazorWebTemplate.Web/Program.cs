using JGUZDV.AspNetCore.Hosting;
using JGUZDV.AspNetCore.Hosting.Components;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

using ZDV.BlazorWebTemplate.Web.Client;

var builder = JGUZDVHostApplicationBuilder.CreateWebHost(
    args, BlazorInteractivityModes.WebAssembly);

builder.Services.AddScoped<CustomCookieAuthenticationEvents>();
builder.Services.AddHttpClient<CustomCookieAuthenticationEvents>(
    client =>
    {
        client.BaseAddress = new Uri(builder.Configuration["ApiBaseURl"]);
    });

builder.Services.Configure<CookieAuthenticationOptions>(CookieAuthenticationDefaults.AuthenticationScheme,
    opt =>
    {
        opt.EventsType = typeof(JGUDirCookieAuthenticationEvents);
        opt.AccessDeniedPath = "/status/access-denied";
    });


builder.Services.Configure<AuthorizationOptions>(authz => authz.AddDefaultPolicies());


var app = builder.BuildAndConfigureBlazor<App>(
    typeof(ZDV.BlazorWebTemplate.Web.Client._Imports).Assembly
    );
await app.RunAsync();
