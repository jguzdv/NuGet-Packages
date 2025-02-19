using JGUZDV.AspNetCore.Hosting;
using JGUZDV.AspNetCore.Hosting.Localization;
using JGUZDV.Blazor.Components.Localization;
using JGUZDV.Blazor.Components.Samples.Web.Components;

var builder = JGUZDVHostApplicationBuilder.CreateWebHost(args, JGUZDV.AspNetCore.Hosting.Components.BlazorInteractivityModes.WebAssembly);
builder.Services.AddScoped<ILanguageService, BlazorLanguageService>();

var app = builder.BuildAndConfigureBlazor<App>(
    additionalBlazorAssemblies: [
        typeof(JGUZDV.Blazor.Components.Samples.Web.Client.Pages.Index).Assembly,
        typeof(JGUZDV.Blazor.Components.Samples.MainLayout).Assembly,
    ]);

app.Run();
