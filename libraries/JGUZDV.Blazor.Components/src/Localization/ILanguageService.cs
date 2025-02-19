using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JGUZDV.Blazor.Components.Localization
{
    /// <summary>
    /// Represents a service that provides language information
    /// </summary>
    public interface ILanguageService
    {
        /// <summary>
        /// Initializes the service - call this before all other methods
        /// </summary>
        public Task InitializeService()
            => Task.CompletedTask;

        /// <summary>
        /// Gets the current culture, e.g. de-DE or de
        /// </summary>
        public string GetCurrentCulture()
            => CultureInfo.CurrentCulture.ToString();

        /// <summary>
        /// Gets the current UI culture, e.g. de-DE or de
        /// </summary>
        public string GetCurrentUICulture()
            => CultureInfo.CurrentUICulture.ToString();

        /// <summary>
        /// Get available languages for a language select.
        /// </summary>
        public List<LanguageItem>? GetLanguages();
    }
}
