using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using JGUZDV.L10n;
using JGUZDV.DynamicForms.Model;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services.AddSupportedCultures(["de","en"]);

builder.Services.AddLocalization();
builder.Services.AddScoped<ValueProvider>();

await builder.Build().RunAsync();
