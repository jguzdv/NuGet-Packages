using System.Security.Claims;
using System.Text.Json.Serialization;

namespace JGUZDV.Extensions.Authorization;

public sealed class ClaimValueRequirement : ClaimRequirement
{
    [JsonConstructor]
    public ClaimValueRequirement(string claimType, string claimValue, 
        bool disableWildcardMatch = false,
        StringComparison claimTypeComparison = StringComparison.OrdinalIgnoreCase,
        StringComparison claimValueComparison = StringComparison.Ordinal)
    {
        ArgumentException.ThrowIfNullOrEmpty(claimType);
        ArgumentException.ThrowIfNullOrEmpty(claimValue);

        ClaimType = claimType;
        ClaimValue = claimValue;
        DisableWildcardMatch = disableWildcardMatch;
        ClaimTypeComparison = claimTypeComparison;
        ClaimValueComparison = claimValueComparison;
    }


    public string ClaimType { get; }
    public string ClaimValue { get; }

    public bool DisableWildcardMatch { get; }
    public StringComparison ClaimTypeComparison { get; }
    public StringComparison ClaimValueComparison { get; }

    public sealed override bool SatisfiesRequirement(ClaimsPrincipal principal)
        =>  principal.HasClaim(c =>
            (c.Type.Equals(ClaimType, ClaimTypeComparison) || (!DisableWildcardMatch && ClaimType.Equals("*"))) &&
            (c.Value.Equals(ClaimValue, ClaimTypeComparison) || (!DisableWildcardMatch && ClaimValue.Equals("*")))
        );
}
