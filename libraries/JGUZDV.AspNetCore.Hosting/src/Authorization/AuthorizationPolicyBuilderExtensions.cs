using JGUZDV.AspNetCore.Hosting.Authorization;

using Microsoft.AspNetCore.Authorization;

namespace JGUZDV.AspNetCore.Hosting;

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
        return builder.AddRequirements(
            new ClaimCollectionAuthorizationRequirement(
                ClaimCollectionAuthorizationRequirement.MatchType.Any,
                scopeClaimType,
                null,
                [allowedScope]
            )
        );
    }

    /// <summary>
    /// Checks the list of scopes in the user's claims to see if any of them match the allowed scopes.
    /// Since scopes can occure either one per claim or multiple scopes in a single claim, this method
    /// splits the contents of the claim by spaces and checks if any of the resulting strings match any of the allowed scopes
    /// </summary>
    public static AuthorizationPolicyBuilder RequireAnyScope(this AuthorizationPolicyBuilder builder, IEnumerable<string> allowedScopes, string scopeClaimType = "scope")
    {
        return builder.AddRequirements(
            new ClaimCollectionAuthorizationRequirement(
                ClaimCollectionAuthorizationRequirement.MatchType.Any,
                scopeClaimType,
                null,
                [.. allowedScopes]
            )
        );
    }

    /// <summary>
    /// Checks the list of scopes in the user's claims to see if all of them match the allowed scopes.
    /// Since scopes can occure either one per claim or multiple scopes in a single claim, this method
    /// splits the contents of the claim by spaces and checks if resulting strings match all of the allowed scopes
    /// </summary>
    public static AuthorizationPolicyBuilder RequireAllScopes(this AuthorizationPolicyBuilder builder, IEnumerable<string> allowedScopes, string scopeClaimType = "scope")
    {
        return builder.AddRequirements(
            new ClaimCollectionAuthorizationRequirement(
                ClaimCollectionAuthorizationRequirement.MatchType.All,
                scopeClaimType,
                null,
                [.. allowedScopes]
            )
        );
    }


}