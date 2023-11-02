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


    public override bool IsSatisfiedBy(IEnumerable<Claim>? claims)
        => claims != null && Requirements.Any() &&
        MatchType switch
        {
            RequirementCollectionMatchType.MatchAll => Requirements.All(r => r.IsSatisfiedBy(claims)),
            RequirementCollectionMatchType.MatchAny => Requirements.Any(r => r.IsSatisfiedBy(claims)),
            _ => false
        };


    public sealed override ClaimRequirementCollection Clone()
    {
        var requirements = new List<ClaimRequirement>();
        foreach (var requirement in Requirements)
        {
            requirements.Add(requirement.Clone());
        }
        var result = new ClaimRequirementCollection(requirements, MatchType);
        return result;
    }

    public sealed override bool Equals(ClaimRequirement? other)
    {
        if (other is not ClaimRequirementCollection c)
            return false;

        return MatchType == c.MatchType &&
            Requirements.Count == c.Requirements.Count &&
            Requirements.SequenceEqual(c.Requirements);
    }
}

public enum RequirementCollectionMatchType
{
    MatchAll,
    MatchAny
}