using System.Security.Claims;
using System.Text.Json.Serialization;

namespace JGUZDV.Extensions.Authorization;

public sealed class ClaimRequirementCollection : ClaimRequirement
{
    [JsonConstructor]
    public ClaimRequirementCollection(List<ClaimRequirement> requirements, RequirementCollectionMatchType matchType)
    {
        Requirements = requirements;
        MatchType = matchType;
    }

    public ClaimRequirementCollection(RequirementCollectionMatchType matchType, params ClaimRequirement[] requirements)
    {
        Requirements = requirements.ToList();
        MatchType = matchType;
    }

    public List<ClaimRequirement> Requirements { get; }
    public RequirementCollectionMatchType MatchType { get; }


    public sealed override bool IsSatisfiedBy(ClaimsPrincipal? principal)
        => principal != null && 
        MatchType switch
        {
            RequirementCollectionMatchType.MatchAll => Requirements.Any() && Requirements.All(r => r.IsSatisfiedBy(principal)),
            RequirementCollectionMatchType.MatchAny => Requirements.Any(r => r.IsSatisfiedBy(principal)),
            _ => false
        };
}

public enum RequirementCollectionMatchType
{
    MatchAll,
    MatchAny
}