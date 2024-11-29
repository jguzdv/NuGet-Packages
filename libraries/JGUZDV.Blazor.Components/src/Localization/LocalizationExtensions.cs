using System.Globalization;

using JGUZDV.Blazor.Components.Localization;

using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;

namespace JGUZDV.Blazor.Components;

/// <summary>
/// Extensions for localization services
/// </summary>
public static class LocalizationExtensions
{
    /// <summary>
    /// Adds localization services to the application and registeres the <see cref="LanguageService"/> and <see cref="LanguageAwareMessageHandler"/>
    /// </summary>
    public static IServiceCollection AddLocalization(this IServiceCollection services, string[] languages, Action<LocalizationOptions>? configureLocalization)
    {
        if (languages is not { Length: >= 1 })
            throw new ArgumentException("At least one language needs to be set", nameof(languages));

        services.AddLocalization(configureLocalization ?? (_ => {}));
        services.AddOptions<LanguageOptions>()
            .Configure(opt => {
                opt.DefaultLanguage = languages[0];
                opt.SupportedLanguages = languages;
            });

        services.TryAddSingleton<LanguageService>();
        services.TryAddScoped<LanguageAwareMessageHandler>();

        return services;
    }

    /// <summary>
    /// Initializes the language for the application. This sould be called upon startup to set the correct culture for the application
    /// </summary>
    public static async Task InitializeLanguageAsync(this WebAssemblyHost host)
    {
        var ls = host.Services.GetRequiredService<LanguageService>();
        var langId = await ls.GetCurrentLanguage();
        var culture = new CultureInfo(langId);

        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;
    }
}
