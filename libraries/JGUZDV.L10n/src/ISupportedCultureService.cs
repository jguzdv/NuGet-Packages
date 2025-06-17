namespace JGUZDV.L10n
{
    /// <summary>
    /// Defines methods and processes regarding the supported cultures.
    /// </summary>
    [Obsolete("Use ILanguageService from JGUZDV.Blazor.Components.L10n")]
    public interface ISupportedCultureService
    {
        /// <summary>
        /// Returns all the supported cultures.
        /// </summary>
        /// <returns>>List of strings including culture shortcuts.</returns>
        List<string> GetSupportedCultures();
    }
}
