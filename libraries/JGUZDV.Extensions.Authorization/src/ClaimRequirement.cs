using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using System.Text.Json.Serialization;

namespace JGUZDV.Extensions.Authorization;

[JsonDerivedType(typeof(ClaimValueRequirement), typeDiscriminator: "Value")]
[JsonDerivedType(typeof(ClaimRequirementCollection), typeDiscriminator: "List")]
[JsonDerivedType(typeof(NullRequirement), typeDiscriminator: "Null")]
public abstract class ClaimRequirement : IEquatable<ClaimRequirement>
{
    public virtual bool IsSatisfiedBy(ClaimsPrincipal? principal)
        => IsSatisfiedBy(principal?.Claims);
        
    public abstract bool IsSatisfiedBy(IEnumerable<Claim>? claims);

    public abstract ClaimRequirement Clone();

    public abstract bool Equals(ClaimRequirement? other);
}
