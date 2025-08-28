using JGUZDV.AspNetCore.Hosting.Localization;

using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Extensions for httpclient builder
    /// </summary>
    public static class HttpClientServiceCollectionExtensions
    {
        /// <summary>
        /// Adds a language header handler to the httpClientBuilder
        /// </summary>
        public static IHttpClientBuilder AddLocalizationHeaderHandler(
            this IHttpClientBuilder httpClientBuilder)
        {
            httpClientBuilder.Services.TryAddTransient<LanguageHeaderHandler>();
            return httpClientBuilder.AddHttpMessageHandler<LanguageHeaderHandler>();
        }
    }
}
