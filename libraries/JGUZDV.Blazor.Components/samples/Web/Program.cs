using System.Security.Claims;

using JGUZDV.AspNetCore.Hosting;
using JGUZDV.Blazor.Components.Samples.Web.Components;
using JGUZDV.Blazor.Components;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = JGUZDVHostApplicationBuilder.CreateWebHost(args, JGUZDV.AspNetCore.Hosting.Components.BlazorInteractivityModes.WebAssembly);

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(x => x.ExpireTimeSpan = TimeSpan.FromMinutes(2));

var app = builder.BuildAndConfigureBlazor<App>(
    additionalBlazorAssemblies: [
        typeof(JGUZDV.Blazor.Components.Samples.Web.Client.Pages._Imports).Assembly,
        typeof(JGUZDV.Blazor.Components.Samples._Imports).Assembly,
    ]);

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


