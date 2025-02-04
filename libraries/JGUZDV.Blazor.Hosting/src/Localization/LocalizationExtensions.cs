using System.Globalization;

using JGUZDV.Blazor.Components.Localization;

using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace JGUZDV.Blazor.Hosting.Localization;

/// <summary>
/// Extensions for localization services
/// </summary>
public static class LocalizationExtensions
{
    /// <summary>
    /// Initializes the language for the application. This sould be called upon startup to set the correct culture for the application
    /// </summary>
    public static async Task InitializeLanguageAsync(this WebAssemblyHost host)
    {
        var ls = host.Services.GetRequiredService<ILanguageService>();
        await ls.InitializeService();

        var uiCulture = new CultureInfo(ls.GetCurrentUICulture());
        var culture = new CultureInfo(ls.GetCurrentUICulture());

        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = uiCulture;
        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = uiCulture;
    }
}
