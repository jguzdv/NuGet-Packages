using System.Security.Claims;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace JGUZDV.AspNetCore.Hosting.Authentication;

/// <summary>
/// Extension methods for mapping authentication endpoints.
/// </summary>
public static class AuthenticationEndpointsExtensions
{
    /// <summary>
    /// Maps sign-in and sign-out endpoints to the route.
    /// </summary>
    /// <param name="route"></param>
    /// <param name="routePrefix"></param>
    /// <returns></returns>
    public static IEndpointRouteBuilder MapAuthentication(this IEndpointRouteBuilder route, string routePrefix = "_app")
    {
        var authN = route.MapGroup(routePrefix);

        authN.MapGet("sign-in",
            (ClaimsPrincipal currentUser, string redirectUri = "/") => currentUser.Identity?.IsAuthenticated != true
                ? Results.Challenge(new AuthenticationProperties { RedirectUri = redirectUri })
                : Results.LocalRedirect(redirectUri));

        authN.MapGet("sign-out",
            (ClaimsPrincipal currentUser, string redirectUri = "/") => currentUser.Identity?.IsAuthenticated == true
                ? Results.SignOut(new AuthenticationProperties { RedirectUri = redirectUri })
                : Results.LocalRedirect(redirectUri));

        return route;
    }
}
