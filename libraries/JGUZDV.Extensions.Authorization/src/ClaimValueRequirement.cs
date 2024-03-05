using System.Security.Claims;
using System.Text.Json.Serialization;

namespace JGUZDV.Extensions.Authorization;

/// <summary>
/// Represents a requirement that is satisfied by a specific claim value and type.
/// </summary>
public sealed class ClaimValueRequirement : ClaimRequirement
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ClaimValueRequirement"/> class.
    /// </summary>
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

    /// <summary>
    /// Initializes a new instance of the <see cref="ClaimValueRequirement"/> class.
    /// </summary>
    public ClaimValueRequirement(string claimType, string claimValue) : this(claimType, claimValue, false) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ClaimValueRequirement"/> class.
    /// </summary>
    public ClaimValueRequirement(string claimType, string claimValue, bool disableWildcardMatch)
        : this(claimType, claimValue, disableWildcardMatch, StringComparison.OrdinalIgnoreCase, StringComparison.Ordinal) { }

    /// <summary>
    /// Gets the claim type that the requirement is satisfied by.
    /// </summary>
    public string ClaimType { get; }

    /// <summary>
    /// Gets the claim value that the requirement is satisfied by.
    /// </summary>
    public string ClaimValue { get; }


    /// <summary>
    /// Gets a value indicating whether wildcard matching is disabled (allows * in ClaimValue or ClaimType).
    /// </summary>
    public bool DisableWildcardMatch { get; }

    /// <summary>
    /// Gets the comparison type for the claim type.
    /// </summary>
    public StringComparison ClaimTypeComparison { get; }

    /// <summary>
    /// Gets the comparison type for the claim value.
    /// </summary>
    public StringComparison ClaimValueComparison { get; }

    /// <inheritdoc/>
    public sealed override bool IsSatisfiedBy(IEnumerable<Claim>? claims)
        => claims?.Any() == true && claims.Any(c =>
            (c.Type.Equals(ClaimType, ClaimTypeComparison) || (!DisableWildcardMatch && ClaimType.Equals("*"))) &&
            (c.Value.Equals(ClaimValue, ClaimValueComparison) || (!DisableWildcardMatch && ClaimValue.Equals("*")))
        );


    /// <inheritdoc/>
    public sealed override ClaimValueRequirement Clone()
    {
        return new ClaimValueRequirement(
            ClaimType, ClaimValue, 
            DisableWildcardMatch, 
            ClaimTypeComparison, ClaimValueComparison);
    }

    /// <inheritdoc/>
    public sealed override bool Equals(ClaimRequirement? other)
    {
        if (other is not ClaimValueRequirement c)
            return false;

        return ClaimType.Equals(c.ClaimType) &&
            ClaimValue.Equals(c.ClaimValue) &&
            DisableWildcardMatch == c.DisableWildcardMatch &&
            ClaimTypeComparison == c.ClaimTypeComparison &&
            ClaimValueComparison == c.ClaimValueComparison;
    }

    /// <summary>
    /// Creates a new instance of the <see cref="ClaimValueRequirement"/> class from the specified claim.
    /// </summary>
    public static ClaimValueRequirement FromClaim(Claim claim, bool allowWildcard = false)
        => new(claim.Type, claim.Value, !allowWildcard, StringComparison.Ordinal, StringComparison.Ordinal);
}
