using System.Security.Claims;
using System.Text.Json.Serialization;

namespace JGUZDV.Extensions.Authorization;

[JsonDerivedType(typeof(ClaimValueRequirement), typeDiscriminator: "Value")]
[JsonDerivedType(typeof(ClaimRequirements), typeDiscriminator: "List")]
public abstract class ClaimRequirement
{
    public abstract bool SatisfiesRequirement(ClaimsPrincipal principal);
}
