using System.Security.Claims;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;

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
                async (HttpContext ctx, IConfiguration configuration) =>
                {
                    string? id_token = null;
                    string? accessToken = null;

                    var showTokens = configuration.GetValue<bool>("Diagnostics:ShowTokens");

                    if (configuration.GetValue<bool>("Diagnostics:ShowTokens"))
                    {
                        var authenticationResult = await ctx.AuthenticateAsync();
                        if(authenticationResult.Succeeded)
                        {
                            id_token = authenticationResult.Properties.GetTokenValue("id_token");
                            accessToken = authenticationResult.Properties.GetTokenValue("auth_token");
                        }
                    }

                    return Results.Ok(new
                    {
                        Identities = ctx.User.Identities.Select(identity => new
                        {
                            IsAuthenticated = identity.IsAuthenticated,
                            IsAuthenticatedType = identity.AuthenticationType,
                            Claims = identity.Claims.Select(c => new
                            {
                                c.Type,
                                c.Value
                            })
                        }),
                        Tokens = !showTokens 
                            ? "Add Diagnostics:ShowTokens=true to configuration to show tokens."
                            : (object)new
                            {
                                id_token,
                                accessToken
                            }
                    });
                });

            return route;
        }
    }
}
