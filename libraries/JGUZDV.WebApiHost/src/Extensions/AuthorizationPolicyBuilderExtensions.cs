using Microsoft.AspNetCore.Authorization;

namespace JGUZDV.WebApiHost.Extensions;

/// <summary>
/// Provides new Require* methods for AuthorizationPolicyBuilder
/// </summary>
public static class AuthorizationPolicyBuilderExtensions
{
    /// <summary>
    /// Checks the list of scopes in the user's claims to see if any of them match the allowed scopes.
    /// Since scopes can occure either one per claim or multiple scopes in a single claim, this method
    /// splits the contents of the claim by spaces and checks if any of the resulting strings match any of the allowed scopes
    /// </summary>
    public static AuthorizationPolicyBuilder RequireScope(this AuthorizationPolicyBuilder builder, string allowedScope, string scopeClaimType = "scope")
    {
        return builder.RequireAssertion(context =>
            context.User.FindAll(scopeClaimType).SelectMany(c => c.Value.Split(' '))
                .Contains(allowedScope, StringComparer.Ordinal)
        );
    }

    /// <summary>
    /// Checks the list of scopes in the user's claims to see if any of them match the allowed scopes.
    /// Since scopes can occure either one per claim or multiple scopes in a single claim, this method
    /// splits the contents of the claim by spaces and checks if any of the resulting strings match any of the allowed scopes
    /// </summary>
    public static AuthorizationPolicyBuilder RequireAnyScope(this AuthorizationPolicyBuilder builder, IEnumerable<string> allowedScopes, string scopeClaimType = "scope")
    {
        return builder.RequireAssertion(context => 
            context.User.FindAll(scopeClaimType).SelectMany(c => c.Value.Split(' '))
                .Intersect(allowedScopes, StringComparer.Ordinal)
                .Any()
        );
    }
}
