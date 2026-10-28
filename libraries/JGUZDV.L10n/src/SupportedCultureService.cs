using JGUZDV.AspNetCore.Components.Localization;

namespace JGUZDV.L10n
{
    /// <summary>
    /// Handles methods and processes regarding the supported cultures.
    /// </summary>
    public class SupportedCultureService : ISupportedCultureService
    {
        private readonly List<string> _supportedCultures = [];

        private readonly ILanguageService? _languageService;

        /// <summary>
        /// Initializes a new instance of the <see cref="SupportedCultureService"/> class with the specified language service.
        /// </summary>
        /// <param name="languageService"></param>
        public SupportedCultureService(ILanguageService languageService)
        {
            _languageService = languageService;
        }

        /// <summary>
        /// Assigns the supported cultures.
        /// </summary>
        /// <param name="supportedCultures"></param>
        public SupportedCultureService(List<string> supportedCultures)
        {
            _supportedCultures = supportedCultures;
        }

        /// <summary>
        /// Returns all the supported cultures.
        /// </summary>
        /// <returns>List of strings including culture shortcuts.</returns>
        /// <exception cref="Exception"></exception>
        public List<string> GetSupportedCultures()
        {
            return _languageService?.GetLanguages().Select(x => x.Culture).ToList() ?? _supportedCultures.ToList();
        }
    }
}