using System.Globalization;

namespace JGUZDV.AspNetCore.Hosting.Localization
{
    /// <summary>
    /// Delegating handler, that adds the current culure and current ui culture to http headers.
    /// </summary>
    public class LanguageHeaderHandler : DelegatingHandler
    {
        /// <inheritdoc />
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            SetAcceptLanguageHeader(request);

            return base.SendAsync(request, cancellationToken);
        }

        /// <inheritdoc />
        protected override HttpResponseMessage Send(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            SetAcceptLanguageHeader(request);

            return base.Send(request, cancellationToken);
        }

        private static void SetAcceptLanguageHeader(HttpRequestMessage request)
        {
            string[] cultures = [CultureInfo.CurrentCulture.Name, CultureInfo.CurrentUICulture.Name];

            var languages = request.Headers.AcceptLanguage.ToList();
            foreach (var c in cultures.Distinct())
            {
                languages.Insert(0, new(c, 1));
            }

            request.Headers.AcceptLanguage.Clear();
            foreach (var headerValue in languages)
            {
                request.Headers.AcceptLanguage.Add(headerValue);
            }
        }
    }
}
