using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using System.Text.Json.Serialization;

namespace JGUZDV.Extensions.Authorization;

/// <summary>
/// Represents a requirement that can be satisfied by a set of claims or a claims principal.
/// </summary>
[JsonDerivedType(typeof(ClaimValueRequirement), typeDiscriminator: "Value")]
[JsonDerivedType(typeof(ClaimRequirementCollection), typeDiscriminator: "List")]
[JsonDerivedType(typeof(NullRequirement), typeDiscriminator: "Null")]
public abstract class ClaimRequirement : IEquatable<ClaimRequirement>
{
    /// <summary>
    /// Determines if the requirement is satisfied by the specified principal.
    /// </summary>
    public virtual bool IsSatisfiedBy(ClaimsPrincipal? principal)
        => IsSatisfiedBy(principal?.Claims);
        
    /// <summary>
    /// Determines if the requirement is satisfied by the specified claims.
    /// </summary>
    public abstract bool IsSatisfiedBy(IEnumerable<Claim>? claims);

    /// <summary>
    /// Deep clones the requirement.
    /// </summary>
    public abstract ClaimRequirement Clone();

    /// <summary>
    /// Determines if the requirement is equal to the specified requirement.
    /// </summary>
    public abstract bool Equals(ClaimRequirement? other);
}
