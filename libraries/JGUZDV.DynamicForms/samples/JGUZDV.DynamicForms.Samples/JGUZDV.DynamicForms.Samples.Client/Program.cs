using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using JGUZDV.L10n;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services.AddSupportedCultures(["de","en"]);

builder.Services.AddLocalization();

await builder.Build().RunAsync();
