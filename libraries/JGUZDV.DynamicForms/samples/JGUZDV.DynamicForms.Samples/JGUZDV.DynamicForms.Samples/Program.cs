using System.Globalization;

using JGUZDV.DynamicForms.Samples.Client.Pages;
using JGUZDV.DynamicForms.Samples.Components;
using JGUZDV.Blazor.Components.L10n;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();
builder.Services.AddLocalization();
builder.Services.AddSupportedCultures(["de", "en"]);
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.UseRequestLocalization("de", "en");

app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(JGUZDV.DynamicForms.Samples.Client._Imports).Assembly);

app.Run();
