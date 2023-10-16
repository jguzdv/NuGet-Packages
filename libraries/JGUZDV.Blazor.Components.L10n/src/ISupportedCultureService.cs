namespace JGUZDV.Blazor.Components.L10n
{
    /// <summary>
    /// Interface used to implement methods for the SupportedCultureService.
    /// </summary>
    public interface ISupportedCultureService
    {
        /// <summary>
        /// Method to get all the supported cultures.
        /// </summary>
        /// <returns></returns>
        List<string> GetSupportedCultures();
    }
}
