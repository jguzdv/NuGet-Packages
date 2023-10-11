using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using JGUZDV.Blazor.Components.Samples;
using JGUZDV.Blazor.Components.Modals;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddModals();
builder.Services.AddAuthorizationCore(options =>
{
    options.AddPolicy("Test", policy => policy.RequireClaim(""));
});

builder.Services.AddLocalization();


var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

await app.RunAsync();
