namespace JGUZDV.Blazor.Components.L10n
{
    public class SupportedCultureService : ISupportedCultureService
    {
        private List<string> _cultures;
        public List<string> GetSupportedCultures()
        {
            return _cultures == null
                ? throw new Exception("The list of supported cultures is null. To fix that issue please set the cultures via the SupportedCultureService method or via parameter in the L10nEditor component.")
                : _cultures;
        }

        public void SetSupportedCultures(List<string> cultures)
        {
            _cultures = cultures;
        }
    }
}
