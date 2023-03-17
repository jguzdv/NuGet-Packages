using System.Security.Claims;
using System.Text.Json.Serialization;

namespace JGUZDV.Extensions.Authorization;

public sealed class ClaimRequirementCollection : ClaimRequirement
{
    [JsonConstructor]
    public ClaimRequirementCollection(List<ClaimRequirement> requirements, ClaimRequirementListMatchType matchType)
    {
        Requirements = requirements;
        MatchType = matchType;
    }

    public List<ClaimRequirement> Requirements { get; }
    public ClaimRequirementListMatchType MatchType { get; }


    public sealed override bool SatisfiesRequirement(ClaimsPrincipal principal)
        => MatchType switch
        {
            ClaimRequirementListMatchType.MatchAll => Requirements.All(r => r.SatisfiesRequirement(principal)),
            ClaimRequirementListMatchType.MatchAny => Requirements.Any(r => r.SatisfiesRequirement(principal)),
            _ => false
        };
}

public enum ClaimRequirementListMatchType
{
    MatchAll,
    MatchAny
}