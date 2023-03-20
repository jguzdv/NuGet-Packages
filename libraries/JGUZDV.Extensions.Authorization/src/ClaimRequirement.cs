using System.Security.Claims;
using System.Text.Json.Serialization;

namespace JGUZDV.Extensions.Authorization;

[JsonDerivedType(typeof(ClaimValueRequirement))]
[JsonDerivedType(typeof(ClaimRequirementCollection), typeDiscriminator: "List")]
public abstract class ClaimRequirement
{
    public abstract bool SatisfiesRequirement(ClaimsPrincipal principal);
}
