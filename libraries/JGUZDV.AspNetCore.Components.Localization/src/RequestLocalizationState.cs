namespace JGUZDV.AspNetCore.Components.Localization;

/// <summary>
/// Represents the state of the request localization.
/// </summary>
public class RequestLocalizationState
{
    /// <summary>
    /// The current culture of the request, e.g. de-DE.
    /// </summary>
    public required string CurrentCulture { get; init; }

    /// <summary>
    /// The current UI culture of the request, e.g. de-DE
    /// </summary>
    public required string CurrentUICulture { get; init; }


    /// <summary>
    /// The supported cultures of the application.
    /// </summary>
    public required LanguageItem[] SupportedCultures { get; init; }

    /// <summary>
    /// The supported UI cultures of the application.
    /// </summary>
    public required LanguageItem[] SupportedUICultures { get; init; }
}