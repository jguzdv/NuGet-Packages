namespace JGUZDV.Blazor.Components.L10n
{
    /// <summary>
    /// Service used to handle methods implemented in the ISupportedCultureService.
    /// </summary>
    public class SupportedCultureService : ISupportedCultureService
    {
        private readonly List<string> _supportedCultures;

        /// <summary>
        /// Constructor of Service to integrate the list of supported cultures.
        /// </summary>
        /// <param name="supportedCultures"></param>
        public SupportedCultureService(List<string> supportedCultures)
        {
            _supportedCultures = supportedCultures;
        }

        /// <summary>
        /// Method to get all the supported cultures.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<string> GetSupportedCultures()
        {
            return _supportedCultures.Any()
                ? _supportedCultures :
                throw new Exception("The list of supported cultures is null. To fix that issue please set the cultures via the SupportedCultureService method or via parameter in the L10nEditor component.");
        }
    }
}