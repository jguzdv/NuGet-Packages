using Microsoft.AspNetCore.Http;

namespace JGUZDV.AspNetCore.Hosting.Extensions
{
    /// <summary>
    /// Provides extension methods for the <see cref="HttpRequest"/> class to simplify common request-related
    /// operations.
    /// </summary>
    public static class HttpRequestExtensions
    {
        /// <summary>
        /// Determines whether the specified HTTP request is likely a request for an HTML page.
        /// </summary>
        /// <remarks>This method evaluates the request headers to determine if the request is intended for
        /// an HTML page.  It checks the following conditions: <list type="bullet"> <item><description>The presence of
        /// the "Sec-Fetch-User" header with a value of "?1".</description></item> <item><description>The presence of
        /// the "Sec-Fetch-Dest" header with a value of "document".</description></item> <item><description>The "Accept"
        /// header includes "text/html".</description></item> </list> If any of these conditions are met, the method
        /// returns <see langword="true"/>.</remarks>
        /// <param name="request">The HTTP request to evaluate. Cannot be <see langword="null"/>.</param>
        /// <returns><see langword="true"/> if the request is likely a request for an HTML page; otherwise, <see
        /// langword="false"/>.</returns>
        public static bool IsHtmlPageRequest(this HttpRequest request)
        {
            // If a user requests the page, then he most likely wants a HTML page.
            var secFetchUserHeader = request.Headers["Sec-Fetch-User"];
            if (secFetchUserHeader.Count > 0 && secFetchUserHeader[0] == "?1")
            {
                return true;
            }

            // If the Sec-Fetch-Dest is "document", then it is likely a request for a HTML page.
            var secFetchDestHeader = request.Headers["Sec-Fetch-Dest"];
            if (secFetchDestHeader.Count > 0 && secFetchDestHeader[0] == "document")
            {
                return true;
            }

            // If the request has an Accept header that includes "text/html", then it is likely a request for a HTML page.
            var acceptHeader = request.Headers["Accept"];
            if (acceptHeader.Count > 0 && acceptHeader[0]!.Contains("text/html"))
            {
                return true;
            }

            return false;
        }
    }
}
