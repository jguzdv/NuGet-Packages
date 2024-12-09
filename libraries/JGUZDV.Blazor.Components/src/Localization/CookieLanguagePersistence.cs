
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;

namespace JGUZDV.Blazor.Components.Localization;

public class CookieLanguageOptions
{
    public string CookieName { get; set; } = ".AspNetCore.Culture";
}

internal class CookieLanguagePersistence : ILanguagePersistence
{
    private readonly string _cookieName;
    private readonly Task<IJSObjectReference> _module;

    public CookieLanguagePersistence(
        IOptions<CookieLanguageOptions> options, 
        IJSRuntime jsRuntime)
    {
        _cookieName = options.Value.CookieName;

        _module = jsRuntime.InvokeAsync<IJSObjectReference>("import", "./_content/JGUZDV.Blazor.Components/js/Localization.js").AsTask();
    }

    public async Task<string?> GetSelectedLanguageAsync()
    {
        var module = await _module;
        return await module.InvokeAsync<string>("readLanguageCookieValue", _cookieName);
    }

    public async Task SaveSelectedLanguageAsync(string? languageId)
    {
        var module = await _module;
        await module.InvokeAsync<string>("setOrReplaceLanguageCookieValue", _cookieName, languageId);
    }
}