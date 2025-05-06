using System.ComponentModel.DataAnnotations;

using JGUZDV.DynamicForms.Resources;
using JGUZDV.L10n;

using Microsoft.Extensions.Localization;

namespace JGUZDV.DynamicForms.Model;

/// <summary>
/// Represents a choice option for a field.
/// </summary>
public class ChoiceOption : IValidatableObject
{
    /// <summary>
    /// Gets or sets the value of the choice option.
    /// </summary>
    public string? Value { get; set; }

    /// <summary>
    /// Gets or sets the name of the choice option.
    /// </summary>
    public L10nString Name { get; set; } = new();

    /// <summary>
    /// Validates the choice option.
    /// </summary>
    /// <param name="validationContext">The validation context.</param>
    /// <returns>A collection of validation results.</returns>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var service = (ISupportedCultureService)validationContext.GetService(typeof(ISupportedCultureService))!;
        var SL = (IStringLocalizer<Validations>)validationContext.GetService(typeof(IStringLocalizer<Validations>))!;

        foreach (var lang in service?.GetSupportedCultures() ?? new())
        {
            if (string.IsNullOrWhiteSpace(Name[lang])) yield return new ValidationResult(SL[$"{nameof(ChoiceOption)}.{nameof(Name)}", lang], new string[] { nameof(Name) });
        }

        if (string.IsNullOrWhiteSpace(Value)) yield return new ValidationResult(SL[$"{nameof(ChoiceOption)}.{nameof(Value)}"], new string[] { nameof(Value) });
    }
}
