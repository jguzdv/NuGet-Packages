using System.Security.Claims;
using System.Text.Json.Serialization;

namespace JGUZDV.Extensions.Authorization;

[JsonDerivedType(typeof(ClaimValueRequirement), typeDiscriminator: "Value")]
[JsonDerivedType(typeof(ClaimRequirementCollection), typeDiscriminator: "List")]
[JsonDerivedType(typeof(NullRequirement), typeDiscriminator: "Null")]
public abstract class ClaimRequirement
{
    public abstract bool IsSatisfiedBy(ClaimsPrincipal? principal);

    public abstract ClaimRequirement Clone();
}
