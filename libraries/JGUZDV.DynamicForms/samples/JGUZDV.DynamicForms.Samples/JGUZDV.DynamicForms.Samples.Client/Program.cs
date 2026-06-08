using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using JGUZDV.L10n;
using JGUZDV.DynamicForms.Extensions;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services.AddSupportedCultures(["de","en"]);

builder.Services.AddLocalization();

builder.Services.AddDynamicForms();

builder.Services.AddHttpClient();

await builder.Build().RunAsync();
