using System.Globalization;

using JGUZDV.AspNetCore.Components.Localization;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;

namespace JGUZDV.AspNetCore.Hosting.Localization;

/// <summary>
/// This service provides the current language settings from the RequestLocalizationOptions.
/// </summary>
public class LanguageService : ILanguageService
{
    private readonly IOptions<RequestLocalizationOptions> _options;

    /// <summary>
    /// Creates a new instance of the LanguageService.
    /// </summary>
    public LanguageService(IOptions<RequestLocalizationOptions> options)
    {
        _options = options;
    }


    /// <inheritdoc />
    public string GetCurrentCulture()
        => CultureInfo.CurrentCulture.ToString();

    /// <inheritdoc />
    public string GetCurrentUICulture()
        => CultureInfo.CurrentUICulture.ToString();

    /// <inheritdoc />
    public IEnumerable<LanguageItem> GetLanguages()
        => _options.Value.SupportedUICultures?.Select(c => new LanguageItem(c.ToString(), c.NativeName)) ?? [];
}
