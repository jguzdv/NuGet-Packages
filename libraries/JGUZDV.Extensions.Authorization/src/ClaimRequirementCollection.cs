using System.Security.Claims;
using System.Text.Json.Serialization;

namespace JGUZDV.Extensions.Authorization;

/// <summary>
/// Represents a collection of requrements that can be combined with either or or and as combination logic.
/// </summary>
public sealed class ClaimRequirementCollection : ClaimRequirement
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ClaimRequirementCollection"/> class.
    /// </summary>
    [JsonConstructor]
    public ClaimRequirementCollection(List<ClaimRequirement> requirements, RequirementCollectionMatchType matchType)
    {
        Requirements = requirements;
        MatchType = matchType;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ClaimRequirementCollection"/> class.
    /// </summary>
    public ClaimRequirementCollection(RequirementCollectionMatchType matchType, params ClaimRequirement[] requirements)
    {
        Requirements = requirements.ToList();
        MatchType = matchType;
    }

    /// <summary>
    /// Gets the requirements in the collection.
    /// </summary>
    public List<ClaimRequirement> Requirements { get; }

    /// <summary>
    /// Gets the match type for the collection.
    /// </summary>
    public RequirementCollectionMatchType MatchType { get; }

    /// <inheritdoc/>
    public override bool IsSatisfiedBy(IEnumerable<Claim>? claims)
        => claims != null && Requirements.Any() &&
        MatchType switch
        {
            RequirementCollectionMatchType.MatchAll => Requirements.All(r => r.IsSatisfiedBy(claims)),
            RequirementCollectionMatchType.MatchAny => Requirements.Any(r => r.IsSatisfiedBy(claims)),
            _ => false
        };

    /// <inheritdoc/>
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

    /// <inheritdoc/>
    public sealed override bool Equals(ClaimRequirement? other)
    {
        if (other is not ClaimRequirementCollection c)
            return false;

        return MatchType == c.MatchType &&
            Requirements.Count == c.Requirements.Count &&
            Requirements.SequenceEqual(c.Requirements);
    }
}

/// <summary>
/// Represents the match type for a collection of requirements.
/// </summary>
public enum RequirementCollectionMatchType
{
    /// <summary>
    /// Requirment can be satisfied if all requirements are satisfied (AND).
    /// </summary>
    MatchAll,

    /// <summary>
    /// Requirement can be satisfied if any requirement is satisfied (OR).
    /// </summary>
    MatchAny
}