using Microsoft.AspNetCore.Http;

namespace JGUZDV.AspNetCore.Hosting.Extensions
{
    internal static class HttpRequestExtensions
    {
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
