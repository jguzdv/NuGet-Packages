using System.Globalization;

using JGUZDV.Blazor.Components.Localization;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;

namespace JGUZDV.AspNetCore.Hosting.Localization
{
    public class BlazorLanguageService : JGUZDV.Blazor.Components.Localization.ILanguageService
    {
        private readonly IOptions<RequestLocalizationOptions> _options;

        public BlazorLanguageService(IOptions<RequestLocalizationOptions> options)
        {
            _options = options;
        }

        /// <inheritdoc />
        public string GetCurrentCulture()
            => CultureInfo.CurrentCulture.ToString();

        /// <inheritdoc />
        public string GetCurrentUICulture()
            => CultureInfo.CurrentUICulture.ToString();

        /// <inheritdoc />
        public List<LanguageItem>? GetLanguages()
            => _options.Value.SupportedCultures?
                .Select(x => new LanguageItem(x.ToString(), x.NativeName))
                .ToList();

        /// <inheritdoc />
        public Task InitializeService()
            => Task.CompletedTask;
    }
}
