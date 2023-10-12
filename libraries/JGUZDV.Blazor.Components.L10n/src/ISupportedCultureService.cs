namespace JGUZDV.Blazor.Components.L10n
{
    public interface ISupportedCultureService
    {
        List<string> GetSupportedCultures();
        void SetSupportedCultures(List<string> cultures);
    }
}
