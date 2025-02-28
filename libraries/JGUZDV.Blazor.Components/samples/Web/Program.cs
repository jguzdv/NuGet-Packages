using JGUZDV.AspNetCore.Hosting;
using JGUZDV.Blazor.Components.Samples.Web.Components;

var builder = JGUZDVHostApplicationBuilder.CreateWebHost(args, JGUZDV.AspNetCore.Hosting.Components.BlazorInteractivityModes.WebAssembly);

var app = builder.BuildAndConfigureBlazor<App>(
    additionalBlazorAssemblies: [
        typeof(JGUZDV.Blazor.Components.Samples.Web.Client.Pages.Index).Assembly,
        typeof(JGUZDV.Blazor.Components.Samples.MainLayout).Assembly,
    ]);

app.Run();
