using System.Globalization;

using JGUZDV.Blazor.Components.Resources;

using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

using Microsoft.JSInterop;

namespace JGUZDV.Blazor.Components.Localization;

/// <summary>
/// Represents a language selection item for display in a language picker.
/// </summary>
public record LanguageSelectItem(string LanguageId, string DisplayName, bool IsSelected);


internal class LanguageService
{
    private readonly IOptions<LanguageOptions> _options;
    private readonly IStringLocalizer<ComponentStrings> _loc;

    private readonly ILanguagePersistence _persistence;

    private string? _currentLanguageId;
    public string[] SupportedLanguages => _options.Value.SupportedLanguages;

    public LanguageService(
        IOptions<LanguageOptions> options,
        ILanguagePersistence persistence,
        IStringLocalizer<ComponentStrings> loc)
    {
        _options = options;
        _persistence = persistence;
        _loc = loc;
    }

    private async Task<IJSObjectReference> InitializeAsync(IJSRuntime jsRuntime)
    {
        var module = await jsRuntime.InvokeAsync<IJSObjectReference>("import", "./_content/ZDV.BlazorComponents/js/Localization/LanguageUtil.js");
        return module;
    }

    public async Task<string> GetCurrentLanguage()
    {
        if (_currentLanguageId != null)
            return _currentLanguageId;

        var options = _options.Value;
        
        var currentLanguage = (await _persistence.GetSelectedLanguageAsync())
            ?? CultureInfo.DefaultThreadCurrentUICulture?.Name;

        if (!string.IsNullOrWhiteSpace(currentLanguage) &&
            _options.Value.SupportedLanguages.Any(x =>
            currentLanguage.StartsWith(x, StringComparison.OrdinalIgnoreCase))
        )
        {
            _currentLanguageId = currentLanguage;
            return _currentLanguageId;
        }
        else
        {
            _currentLanguageId = options.DefaultLanguage;
            return _currentLanguageId;
        }
    }

    public async Task SetCurrentLanguage(string? languageId)
    {
        var options = _options.Value;

        if (!string.IsNullOrWhiteSpace(languageId) &&
            _options.Value.SupportedLanguages.Contains(languageId))
        {
            _currentLanguageId = languageId;
            await _persistence.SaveSelectedLanguageAsync(languageId);
        }
        else
        {
            _currentLanguageId = options.DefaultLanguage;
            await _persistence.ResetSelectedLanguageAsync();
        }
    }

    public string GetLanguageDisplayName(string lang)
    {
        return _loc[$"LanguagePicker.NativeName:{lang}"]!;
    }
}