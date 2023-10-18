using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using JGUZDV.Blazor.Components.Samples;
using JGUZDV.Blazor.Components.L10n;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddModals();
builder.Services.AddSupportedCultures(new List<string>() { "de", "en" });

builder.Services.AddLocalization();


var app = builder.Build();

await app.RunAsync();
