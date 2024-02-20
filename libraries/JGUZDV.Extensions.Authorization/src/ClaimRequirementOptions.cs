using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace JGUZDV.Extensions.Authorization;

/// <summary>
/// Represents a requirement compatible with the aspnet core options pattern.
/// </summary>
public class ClaimRequirementOptions : IValidatableObject
{
    /// <summary>
    /// Gets or sets the match type for the collection (use with Requirements to create a ClaimRequirementCollection).
    /// </summary>
    public RequirementCollectionMatchType MatchType { get; set; } = RequirementCollectionMatchType.MatchAny;

    /// <summary>
    /// Gets or sets the requirements in the collection (use with MatchType to create a ClaimRequirementCollection).
    /// </summary>
    public List<ClaimRequirementOptions>? Requirements { get; set; }


    /// <summary>
    /// Gets or sets a single claim type (use with ClaimValue to create a ClaimValueRequirement).
    /// </summary>
    public string? ClaimType { get; set; }

    /// <summary>
    /// Gets or sets a single claim value (use with ClaimType to create a  ClaimValueRequirement).
    /// </summary>
    public string? ClaimValue { get; set; }


    /// <summary>
    /// Gets or sets a value indicating whether wildcard matching is disabled.
    /// </summary>
    public bool DisableWildcardMatch { get; set; } = false;

    /// <summary>
    /// Gets or sets the comparison type for the claim type.
    /// </summary>
    public StringComparison ClaimTypeComparison { get; set; } = StringComparison.OrdinalIgnoreCase;

    /// <summary>
    /// Gets or sets the comparison type for the claim value.
    /// </summary>
    public StringComparison ClaimValueComparison { get; set; } = StringComparison.Ordinal;


    private ClaimRequirement? _claimRequirement;

    /// <summary>
    /// Gets the claim requirement represented by the options.
    /// - Either a ClaimRequirementCollection, if Requirements are defined
    /// - Or a ClaimValueRequirement, if ClaimType and ClaimValue are defined
    /// </summary>
    public ClaimRequirement ClaimRequirement { 
        get
        {
            if (_claimRequirement != null)
                return _claimRequirement;

            if(Validate(new ValidationContext(this)).Any())
                throw new InvalidOperationException("Could not successfully validate options.");

            if(Requirements != null)
            {
                _claimRequirement = new ClaimRequirementCollection(
                    Requirements.Select(x => x.ClaimRequirement).ToList(),
                    MatchType);
            } 
            else
            {
                _claimRequirement = new ClaimValueRequirement(
                    ClaimType!, ClaimValue!, DisableWildcardMatch, ClaimTypeComparison, ClaimValueComparison
                );
            }

            return _claimRequirement;
        }
    }

    /// <summary>
    /// Determines if the requirement is satisfied by the specified principal.
    /// </summary>
    public bool SatisfiesRequirement(ClaimsPrincipal principal) => ClaimRequirement.IsSatisfiedBy(principal);

    /// <summary>
    /// Validates the options.
    /// </summary>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Requirements != null) {
            if (ClaimType != null || ClaimValue != null)
                yield return new ValidationResult("Cannot define both Requirements and ClaimType/ClaimValue");

            foreach (var validationResult in Requirements.SelectMany(x => x.Validate(validationContext)))
                yield return validationResult;
        }
        else
        {
            if (ClaimType == null || ClaimValue == null)
                yield return new ValidationResult("Both ClaimType and ClaimValue must be defined");
        }
    }
}
