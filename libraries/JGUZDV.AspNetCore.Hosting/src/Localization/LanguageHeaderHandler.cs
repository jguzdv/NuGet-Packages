using System.Globalization;

namespace JGUZDV.AspNetCore.Hosting.Localization
{
    /// <summary>
    /// Delegating handler, that adds the current culure and current ui culture to http headers.
    /// </summary>
    public class LanguageHeaderHandler : DelegatingHandler
    {
        /// <inheritdoc />
        protected override HttpResponseMessage Send(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var languages = request.Headers.AcceptLanguage.ToList();
            languages.Insert(0, new(CultureInfo.CurrentCulture.Name, 1));
            languages.Insert(0, new(CultureInfo.CurrentUICulture.Name, 1));

            request.Headers.AcceptLanguage.Clear();
            foreach (var headerValue in languages)
            {
                request.Headers.AcceptLanguage.Add(headerValue);
            }

            return base.Send(request, cancellationToken);
        }
    }
}
