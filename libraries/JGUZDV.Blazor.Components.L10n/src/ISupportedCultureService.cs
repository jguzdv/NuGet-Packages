namespace JGUZDV.Blazor.Components.L10n
{
    /// <summary>
    /// Defines methods and processes regarding the supported cultures.
    /// </summary>
    public interface ISupportedCultureService
    {
        /// <summary>
        /// Returns all the supported cultures.
        /// </summary>
        /// <returns>>List of strings including culture shortcuts.</returns>
        List<string> GetSupportedCultures();
    }
}
