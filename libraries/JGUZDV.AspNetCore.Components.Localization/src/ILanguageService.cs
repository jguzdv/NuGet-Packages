namespace JGUZDV.AspNetCore.Components.Localization;

/// <summary>
/// Represents a service that provides language information
/// </summary>
public interface ILanguageService
{
    /// <summary>
    /// Gets the current culture, e.g. de-DE or de
    /// </summary>
    public string GetCurrentCulture();

    /// <summary>
    /// Gets the current UI culture, e.g. de-DE or de
    /// </summary>
    public string GetCurrentUICulture();

    /// <summary>
    /// Get available languages for a language select.
    /// </summary>
    public IEnumerable<LanguageItem>? GetLanguages();
}
