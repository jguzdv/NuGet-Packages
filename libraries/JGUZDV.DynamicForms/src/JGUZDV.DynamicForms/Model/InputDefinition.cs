using System.ComponentModel.DataAnnotations;

using JGUZDV.DynamicForms.Resources;
using JGUZDV.L10n;

using Microsoft.Extensions.Localization;

namespace JGUZDV.DynamicForms.Model;

/// <summary>
/// Represents the definition of an input field.
/// </summary>
public class InputDefinition : IValidatableObject
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InputDefinition"/> class.
    /// </summary>
    public InputDefinition()
    {


        Name = Guid.NewGuid().ToString();
        Id = Name;
    }

    /// <summary>
    /// Gets or sets the label of the input field.
    /// </summary>
    public L10nString Label { get; set; } = new();

    /// <summary>
    /// Gets or sets the name of the input field.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the ID of the input field.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// Validates the input field.
    /// </summary>
    /// <param name="validationContext">The validation context.</param>
    /// <returns>A collection of validation results.</returns>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var service = (ISupportedCultureService)validationContext.GetService(typeof(ISupportedCultureService))!;
        var SL = (IStringLocalizer<Validations>)validationContext.GetService(typeof(IStringLocalizer<Validations>))!;

        foreach (var lang in service?.GetSupportedCultures() ?? new())
        {
            if (string.IsNullOrWhiteSpace(Label[lang])) yield return new ValidationResult(SL[$"{nameof(InputDefinition)}.{nameof(Label)}", lang], new string[] { nameof(Label) });
        }
    }
}
