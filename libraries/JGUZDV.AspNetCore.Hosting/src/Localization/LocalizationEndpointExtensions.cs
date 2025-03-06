using Azure;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Routing;

namespace JGUZDV.AspNetCore.Hosting.Localization;

/// <summary>
/// Extension methods for mapping Localization endpoints.
/// </summary>
public static class LocalizationEndpointExtensions
{
    /// <summary>
    /// Maps the Localization endpoints to the route.
    /// </summary>
    public static IEndpointRouteBuilder MapLocalization(this IEndpointRouteBuilder route, string routePrefix = "_app/localization")
    {
        var localizationApi = route.MapGroup(routePrefix);

        localizationApi.Map("{culture}",
            (
                HttpContext context, 
                string culture,
                string? returnUrl
            ) => {
                context.Response.Cookies.Append(
                    CookieRequestCultureProvider.DefaultCookieName,
                    CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                    new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
                );

                var actualReturnUrl = string.IsNullOrWhiteSpace(returnUrl) ? "/" : returnUrl;
                return Results.LocalRedirect(actualReturnUrl);
            });

        return route;
    }
}
