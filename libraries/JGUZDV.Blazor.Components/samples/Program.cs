using System.Security.Claims;

using JGUZDV.Blazor.Components;
using JGUZDV.Blazor.Components.L10n;
using JGUZDV.Blazor.Components.Samples;

using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddSupportedCultures(new List<string>() { "de", "en" });

builder.Services.AddLocalization(["de", "en", "fr"]);
builder.Services.AddBrowserClientStore();

builder.Services.AddSingleton<AuthenticationStateProvider, SampleAuthenticationStateProvider>();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthorizationCore();

var app = builder.Build();

await app.RunAsync();


internal class SampleAuthenticationStateProvider : AuthenticationStateProvider
{
    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        return Task.FromResult(
            new AuthenticationState(
                new ClaimsPrincipal(
                    new ClaimsIdentity(new List<Claim>
                    {
                        new("name", "Mustermann, Max")
                    },
                    "SampleAuth", "name", "role")
                )
            )
        );
    }
}
