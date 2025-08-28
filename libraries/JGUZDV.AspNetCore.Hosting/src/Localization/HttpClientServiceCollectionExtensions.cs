using JGUZDV.AspNetCore.Hosting.Localization;

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
            => httpClientBuilder.AddHttpMessageHandler<LanguageHeaderHandler>();
    }
}
