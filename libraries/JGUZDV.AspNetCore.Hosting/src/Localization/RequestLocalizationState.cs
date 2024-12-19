
using System.Globalization;

using Microsoft.AspNetCore.Builder;

namespace JGUZDV.AspNetCore.Hosting.Localization;

/// <summary>
/// Represents the state of the request localization.
/// </summary>
public class RequestLocalizationState
{
    internal static RequestLocalizationState FromOptions(RequestLocalizationOptions options)
        => new RequestLocalizationState
        {
            CurrentCulture = CultureInfo.CurrentCulture.ToString(),
            CurrentUICulture = CultureInfo.CurrentUICulture.ToString(),

            SupportedCultures = [.. options.SupportedCultures?.Select(c => new LocalizationInfo(c.ToString(), c.NativeName))],
            SupportedUICultures = [.. options.SupportedUICultures?.Select(c => new LocalizationInfo(c.ToString(), c.NativeName))]
        };


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
    public required LocalizationInfo[] SupportedCultures { get; init; }

    /// <summary>
    /// The supported UI cultures of the application.
    /// </summary>
    public required LocalizationInfo[] SupportedUICultures { get; init; }
}
