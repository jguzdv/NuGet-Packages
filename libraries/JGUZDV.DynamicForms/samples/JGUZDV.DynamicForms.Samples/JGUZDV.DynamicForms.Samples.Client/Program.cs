using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using JGUZDV.L10n;
using JGUZDV.DynamicForms.Model;
using JGUZDV.Blazor.Components;
using JGUZDV.DynamicForms.Blazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services.AddSupportedCultures(["de","en"]);

builder.Services.AddLocalization();
builder.Services.AddScoped<ValueProvider>();
builder.Services.AddToasts();
builder.Services.AddScoped<IFileService, DefaultFileService>();
builder.Services.AddHttpClient();

await builder.Build().RunAsync();
