namespace JGUZDV.Blazor.Components.Localization
{
    /// <summary>
    /// Represents a language selection item
    /// </summary>
    /// <param name="Culture">The culture identifier, e.g. de-DE</param>
    /// <param name="DisplayName">The (native) display name of the language.</param>
    public record LanguageItem(string Culture, string DisplayName);
}