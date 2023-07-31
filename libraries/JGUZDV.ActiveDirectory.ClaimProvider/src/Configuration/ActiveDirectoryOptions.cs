using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace JGUZDV.ActiveDirectory.ClaimProvider.Configuration;

public class ActiveDirectoryOptions : IValidatableObject
{
    [NotNull]
    public ActiveDirectoryConnectionOptions Connection { get; set; } = null!;

    [NotNull]
    public string? UserClaimType { get; set; } = null!;

    [NotNull]
    public string? UserFilter { get; set; } = null!;


    public List<ClaimSource> ClaimSources { get; set; } = new(Defaults.KnownClaimSources);
    public Dictionary<string, string> PropertyConverters { get; set; } = new(Defaults.KnownConverters);


    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Connection == null)
            yield return new ValidationResult("Connection MUST be configured.");
        if (string.IsNullOrWhiteSpace(Connection?.Server))
            yield return new ValidationResult("Connection:Server MUST be configured.");

        if (string.IsNullOrWhiteSpace(UserClaimType))
            yield return new ValidationResult("UserClaimType MUST be configured.");
        if (string.IsNullOrWhiteSpace(UserFilter))
            yield return new ValidationResult("UserFilter MUST be configured.");

        foreach (var claimMap in ClaimSources)
        {
            if (string.IsNullOrWhiteSpace(claimMap.ClaimType))
                yield return new ValidationResult("ClaimMaps:ClaimType MUST be configured");
            if (string.IsNullOrWhiteSpace(claimMap.PropertyName))
                yield return new ValidationResult("ClaimMaps:PropertyName MUST be configured");
        }
    }
}
