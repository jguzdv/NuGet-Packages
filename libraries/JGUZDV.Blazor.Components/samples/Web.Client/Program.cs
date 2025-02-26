using System.Security.Claims;

using JGUZDV.AspNetCore.Components.Localization;
using JGUZDV.Blazor.Components.Localization;
using JGUZDV.Blazor.Hosting;

using Microsoft.AspNetCore.Components.Authorization;

var builder = JGUZDVWebAssemblyApplicationBuilder.CreateDefault(args);

//builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

//builder.Services.AddSupportedCultures(new List<string>() { "de", "en" });

//builder.Services.AddLocalization();
//builder.Services.AddBrowserClientStore();

//builder.Services.AddSingleton<AuthenticationStateProvider, SampleAuthenticationStateProvider>();
//builder.Services.AddSingleton<ILanguageService, SampleLanguageService>();

//builder.Services.AddCascadingAuthenticationState();
//builder.Services.AddAuthorizationCore();


await builder.Build().RunAsync();


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