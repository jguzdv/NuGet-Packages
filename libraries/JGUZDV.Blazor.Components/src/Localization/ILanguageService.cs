using System;
using System.Collections.Generic;
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
        public Task InitializeService();

        /// <summary>
        /// Gets the current culture, e.g. de-DE or de
        /// </summary>
        public string GetCurrentCulture();

        /// <summary>
        /// Gets the current UI culture, e.g. de-DE or de
        /// </summary>
        public string GetCurrentUICulture();

        /// <summary>
        /// Get available languages for a language select.
        /// </summary>
        public List<LanguageItem>? GetLanguages();
    }
}
