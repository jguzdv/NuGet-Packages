using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using JGUZDV.Blazor.Components.L10n;
using System.Resources;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services.AddSupportedCultures(["de","en"]);

builder.Services.AddLocalization();

await builder.Build().RunAsync();
