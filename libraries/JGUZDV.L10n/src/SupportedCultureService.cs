namespace JGUZDV.L10n
{
    /// <summary>
    /// Handles methods and processes regarding the supported cultures.
    /// </summary>
    public class SupportedCultureService : ISupportedCultureService
    {
        private readonly List<string> _supportedCultures;

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
            return _supportedCultures.Any()
                ? _supportedCultures :
                throw new Exception("The list of supported cultures is null. To fix that issue please set the cultures via the SupportedCultureService method or via parameter in the L10nEditor component.");
        }
    }
}