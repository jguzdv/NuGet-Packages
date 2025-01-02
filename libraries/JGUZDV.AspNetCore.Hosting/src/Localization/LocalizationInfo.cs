namespace JGUZDV.AspNetCore.Hosting.Localization;

/// <summary>
/// Represents a language and a language value for localization selection and similar purpose.
/// </summary>
/// <param name="Value">The CultureInfo in form de-DE or LanguageInfo in form de.</param>
/// <param name="NativeDisplayName">The native language display name</param>
public record LocalizationInfo(string Value, string NativeDisplayName);