using JGUZDV.AspNetCore.Hosting;
using JGUZDV.Blazor.Components.Samples.Web.Components;

var builder = JGUZDVHostApplicationBuilder.CreateWebHost(args, JGUZDV.AspNetCore.Hosting.Components.BlazorInteractivityModes.WebAssembly);

var app = builder.BuildAndConfigureBlazor<App>(typeof(JGUZDV.Blazor.Components.Samples._Imports).Assembly);

app.Run();
