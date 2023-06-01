using System.Security.Claims;
using System.Text.Json.Serialization;

namespace JGUZDV.Extensions.Authorization;

[JsonDerivedType(typeof(ClaimValueRequirement), typeDiscriminator: "Value")]
[JsonDerivedType(typeof(ClaimRequirementCollection), typeDiscriminator: "List")]
public abstract class ClaimRequirement
{
    public abstract bool IsSatisfiedBy(ClaimsPrincipal? principal);
}
