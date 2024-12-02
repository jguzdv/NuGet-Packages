namespace JGUZDV.Blazor.Components.Localization;

/// <summary>
/// Priovides a way to persist the selected language
/// </summary>
public interface ILanguagePersistence
{
    /// <summary>
    /// Returns the selected language or null if no language is selected.
    /// </summary>
    Task<string?> GetSelectedLanguageAsync();

    /// <summary>
    /// Saves the selected language.
    /// </summary>
    /// <param name="languageId">
    /// The language id to be stored - can be null to clear the selected language
    /// </param>
    Task SaveSelectedLanguageAsync(string? languageId);

    /// <summary>
    /// Resets the selected language.
    /// </summary>
    Task ResetSelectedLanguageAsync()
        => SaveSelectedLanguageAsync(null);
}
