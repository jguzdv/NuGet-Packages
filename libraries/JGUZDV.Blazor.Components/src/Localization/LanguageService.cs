using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using JGUZDV.Blazor.Components.Resources;

using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

using Microsoft.JSInterop;

namespace JGUZDV.Blazor.Components.Localization;

public record LanguageSelectItem(string LanguageId, string DisplayName, bool IsSelected);

public class LanguageService
{
    private readonly IOptions<LanguageOptions> _options;
    private readonly IStringLocalizer<ComponentStrings> _loc;
    private readonly Task<IJSObjectReference> _jsLanguageService;

    private string? _currentLanguageId;

    public string[] SupportedLanguages => _options.Value.SupportedLanguages;

    public LanguageService(
        IOptions<LanguageOptions> options,
        IStringLocalizer<ComponentStrings> loc,
        IJSRuntime jsRuntime)
    {
        _options = options;
        _loc = loc;
        _jsLanguageService = InitializeAsync(jsRuntime);
    }

    private async Task<IJSObjectReference> InitializeAsync(IJSRuntime jsRuntime)
    {
        var module = await jsRuntime.InvokeAsync<IJSObjectReference>("import", "./_content/ZDV.BlazorComponents/js/Language/LanguageService.js");
        var jsLanguageService = await module.InvokeAsync<IJSObjectReference>("createLanguageService");

        return jsLanguageService;
    }

    public async Task<string> GetCurrentLanguage()
    {
        if (_currentLanguageId != null)
            return _currentLanguageId;

        var options = _options.Value;
        var js = await _jsLanguageService;

        var currentLanguage = (await js.InvokeAsync<string?>("getLanguage"))
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
        var js = await _jsLanguageService;

        if (!string.IsNullOrWhiteSpace(languageId) &&
            _options.Value.SupportedLanguages.Contains(languageId))
        {
            _currentLanguageId = languageId;
            await js.InvokeVoidAsync("setLanguage", languageId);
        }
        else
        {
            _currentLanguageId = options.DefaultLanguage;
            await js.InvokeVoidAsync("setLanguage");
        }
    }

    public string GetLanguageDisplayName(string lang)
    {
        return _loc[$"LanguagePicker.NativeName:{lang}"]!;
    }
}