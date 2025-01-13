using System.Globalization;

using JGUZDV.AspNetCore.Hosting.Localization;
using JGUZDV.Blazor.Components.Localization;

using Microsoft.AspNetCore.Components;

namespace JGUZDV.Blazor.Hosting.Localization;

/// <summary>
/// This service provides the current language settings from the PersistentComponentState.
/// </summary>
public class PersistentStateLanguageService : ILanguageService
{
    private readonly PersistentComponentState _applicationState;
    private RequestLocalizationState? _state;

    /// <summary>
    /// Creates a new instance of the PersistentStateLanguageService.
    /// </summary>
    public PersistentStateLanguageService(PersistentComponentState applicationState)
    {
        _applicationState = applicationState;
    }

    /// <inheritdoc />
    public string GetCurrentCulture()
        => _state?.CurrentCulture ?? CultureInfo.CurrentCulture.ToString();

    /// <inheritdoc />
    public string GetCurrentUICulture() 
        => _state?.CurrentUICulture ?? CultureInfo.CurrentUICulture.ToString();


    /// <inheritdoc />
    public List<LanguageItem>? GetLanguages()
    {
        var currentUICulture = GetCurrentUICulture();

        var result = _state?.SupportedCultures?
            .Select(c => new LanguageItem(c.Value, c.NativeDisplayName))
            .ToList();

        return result;
    }

    /// <inheritdoc />
    public Task InitializeService()
    {
        if(!_applicationState.TryTakeFromJson(nameof(RequestLocalizationState), out _state))
        {
            _state = null;
        }

        return Task.CompletedTask;
    }
}
