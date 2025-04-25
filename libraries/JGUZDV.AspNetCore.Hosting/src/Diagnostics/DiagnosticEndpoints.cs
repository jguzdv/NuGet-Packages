using System.Security.Claims;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace JGUZDV.AspNetCore.Hosting.Diagnostics
{
    /// <summary>
    /// Extension methods for mapping authentication endpoints.
    /// </summary>
    public static class DiagnosticEndpointsExtensions
    {
        /// <summary>
        /// Maps _diag endpoints to the route (_diag/user).
        /// </summary>
        public static IEndpointRouteBuilder MapDiagnostics(this IEndpointRouteBuilder route, string routePrefix = "_app/_diag")
        {
            var diagnostics = route.MapGroup(routePrefix)
                .ExcludeFromDescription();

            diagnostics.MapGet("user",
                (ClaimsPrincipal currentUser) =>
                {
                    return Results.Ok(new
                    {
                        Identities = currentUser.Identities.Select(identity => new
                        {
                            IsAuthenticated = identity.IsAuthenticated,
                            IsAuthenticatedType = identity.AuthenticationType,
                            Claims = identity.Claims.Select(c => new
                            {
                                c.Type,
                                c.Value
                            })
                        })
                    });
                });

            return route;
        }
    }
}
