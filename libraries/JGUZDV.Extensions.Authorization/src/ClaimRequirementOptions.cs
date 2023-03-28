using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace JGUZDV.Extensions.Authorization;

public class ClaimRequirementOptions : IValidatableObject
{
    public RequirementCollectionMatchType MatchType { get; set; } = RequirementCollectionMatchType.MatchAny;
    public List<ClaimRequirementOptions>? Requirements { get; set; }


    public string? ClaimType { get; set; }
    public string? ClaimValue { get; set; }


    public bool DisableWildcardMatch { get; set; } = false;
    public StringComparison ClaimTypeComparison { get; set; } = StringComparison.OrdinalIgnoreCase;
    public StringComparison ClaimValueComparison { get; set; } = StringComparison.Ordinal;


    private ClaimRequirement? _claimRequirement;
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

    public bool SatisfiesRequirement(ClaimsPrincipal principal) => ClaimRequirement.SatisfiesRequirement(principal);


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
