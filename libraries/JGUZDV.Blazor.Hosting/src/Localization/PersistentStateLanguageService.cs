using System.Globalization;

using JGUZDV.AspNetCore.Components.Localization;

using Microsoft.AspNetCore.Components;

namespace JGUZDV.Blazor.Hosting.Localization;

/// <summary>
/// This service provides the current language settings from the PersistentComponentState.
/// </summary>
public class PersistentStateLanguageService : ILanguageService
{
    private readonly PersistentComponentState _applicationState;
    private RequestLocalizationState? _state;

    private RequestLocalizationState? State
    {
        get {
            if(_state == null)
            {
                if (!_applicationState.TryTakeFromJson(nameof(RequestLocalizationState), out _state))
                {
                    _state = null;
                }
            }

            return _state;
        }
    }

    /// <summary>
    /// Creates a new instance of the PersistentStateLanguageService.
    /// </summary>
    public PersistentStateLanguageService(PersistentComponentState applicationState)
    {
        _applicationState = applicationState;
    }

    /// <inheritdoc />
    public string GetCurrentCulture()
        => State?.CurrentCulture ?? CultureInfo.CurrentCulture.ToString();

    /// <inheritdoc />
    public string GetCurrentUICulture() 
        => State?.CurrentUICulture ?? CultureInfo.CurrentUICulture.ToString();


    /// <inheritdoc />
    public IEnumerable<LanguageItem>? GetLanguages()
        => State?.SupportedCultures;
}
