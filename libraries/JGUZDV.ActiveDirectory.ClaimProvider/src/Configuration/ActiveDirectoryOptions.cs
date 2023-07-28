using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace JGUZDV.ActiveDirectory.ClaimProvider.Configuration;

public class ActiveDirectoryOptions : IValidatableObject
{
    public ActiveDirectoryOptions()
    {
        ClaimMaps = new List<ClaimSource>();
    }

    [NotNull]
    public string? UserClaimType { get; set; } = "sub";

    [NotNull]
    public string? UserFilter { get; set; } = "(objectGuid={0})";

    [NotNull]
    public ActiveDirectoryConnectionOptions Connection { get; set; } = null!;

    public List<ClaimSource> ClaimMaps { get; set; } = new();
    public Dictionary<string, string> PropertyConverters { get; set; } = new(Defaults.KnownConverters);

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Connection == null)
            yield return new ValidationResult("Connection MUST be present");
        if (string.IsNullOrWhiteSpace(Connection?.Server))
            yield return new ValidationResult("Connection:Server MUST be present");

        foreach (var claimMap in ClaimMaps)
        {
            if (string.IsNullOrWhiteSpace(claimMap.ClaimType))
                yield return new ValidationResult("ClaimMaps:ClaimType MUST be present");
            if (string.IsNullOrWhiteSpace(claimMap.PropertyName))
                yield return new ValidationResult("ClaimMaps:PropertyName MUST be present");
        }
    }
}
