using System.Security.Claims;
using System.Text.Json.Serialization;

namespace JGUZDV.Extensions.Authorization;

public sealed class ClaimValueRequirement : ClaimRequirement
{
    [JsonConstructor]
    public ClaimValueRequirement(string claimType, string claimValue, 
        bool disableWildcardMatch,
        StringComparison claimTypeComparison,
        StringComparison claimValueComparison)
    {
        ArgumentException.ThrowIfNullOrEmpty(claimType);
        ArgumentException.ThrowIfNullOrEmpty(claimValue);

        ClaimType = claimType;
        ClaimValue = claimValue;
        DisableWildcardMatch = disableWildcardMatch;
        ClaimTypeComparison = claimTypeComparison;
        ClaimValueComparison = claimValueComparison;
    }

    public ClaimValueRequirement(string claimType, string claimValue) : this(claimType, claimValue, false) { }
    
    public ClaimValueRequirement(string claimType, string claimValue, bool disableWildcardMatch) 
        : this(claimType, claimValue, disableWildcardMatch, StringComparison.OrdinalIgnoreCase, StringComparison.Ordinal) { }

    public string ClaimType { get; }
    public string ClaimValue { get; }

    public bool DisableWildcardMatch { get; }
    public StringComparison ClaimTypeComparison { get; }
    public StringComparison ClaimValueComparison { get; }

    public sealed override bool IsSatisfiedBy(ClaimsPrincipal principal)
        =>  principal.HasClaim(c =>
            (c.Type.Equals(ClaimType, ClaimTypeComparison) || (!DisableWildcardMatch && ClaimType.Equals("*"))) &&
            (c.Value.Equals(ClaimValue, ClaimValueComparison) || (!DisableWildcardMatch && ClaimValue.Equals("*")))
        );
}
