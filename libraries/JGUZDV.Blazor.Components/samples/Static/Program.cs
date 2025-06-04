using System.Security.Claims;
using JGUZDV.AspNetCore.Hosting;
using JGUZDV.Blazor.Components.Samples.Static.Components;
using Microsoft.AspNetCore.Authentication.Cookies;

using Microsoft.AspNetCore.Authentication;

var builder = JGUZDVHostApplicationBuilder.CreateStaticWeb(args);

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(x => x.ExpireTimeSpan = TimeSpan.FromMinutes(2));

var app = builder.BuildAndConfigureBlazor<App>();

app.MapGet("_app/sign-in", (HttpContext context, string redirectUri) =>
{
    context.SignInAsync(
        new ClaimsPrincipal(
            new ClaimsIdentity(
                [new("sub", "ABCDF"), new("name", "Gutenberg, Johannes")],
                "Fake Login",
                "sub",
                "role"
        )));

    return Results.LocalRedirect(redirectUri);
});

app.Run();
