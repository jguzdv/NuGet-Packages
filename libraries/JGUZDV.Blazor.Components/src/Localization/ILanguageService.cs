using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JGUZDV.Blazor.Components.Localization
{
    public interface ILanguageService
    {
        public Task InitializeService();

        public string GetCurrentCulture();
        public string GetCurrentUICulture();

        public List<LanguageSelectItem>? GetLanguages();
    }
}
