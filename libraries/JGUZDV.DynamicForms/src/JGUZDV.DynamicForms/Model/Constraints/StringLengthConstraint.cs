using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Data;

using JGUZDV.L10n;

namespace JGUZDV.DynamicForms.Model.Constraints;

/// <summary>
/// Constraint for validating the length of strings.
/// </summary>
public class StringLengthConstraint : IConstraint
{
    /// <inheritdoc />
    public static ConstraintId ConstraintId => new(nameof(StringLengthConstraint));

    /// <inheritdoc />
    public ConstraintId GetConstraintId() => ConstraintId;

    /// <inheritdoc />
    public static L10nString DisplayName => new() { ["de"] = "Textlänge", ["en"] = "Text length" };

    /// <inheritdoc />
    public L10nString GetDisplayName() => DisplayName;



    /// <summary>
    /// The maximum length of the string.
    /// </summary>
    public int MaxLength { get; set; }


    /// <summary>
    /// Validates the constraint.
    /// </summary>
    /// <param name="validationContext">The validation context.</param>
    /// <returns>A collection of validation results.</returns>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (MaxLength < 0)
        {
            yield return new ValidationResult($"{nameof(MaxLength)} may not be negative");
        }
    }

    /// <summary>
    /// Validates the values against the string length constraint.
    /// </summary>
    /// <param name="values">The values to validate.</param>
    /// <param name="context">The validation context.</param>
    /// <returns>A collection of validation results.</returns>
    public IEnumerable<ValidationResult> ValidateConstraint(string[] values, ValidationContext context)
    {
        var fields = new List<string?> { (context.ObjectInstance as FieldDefinition)?.InputDefinition.Name }.Where(x => x != null).ToList();

        return values.Where(value => value.Count() > MaxLength).Select(x => new ValidationResult("Constraint.Validation.Length", fields!));
    }

    /// <summary>
    /// Validates the values against the string length constraint.
    /// </summary>
    /// <param name="values">The values to validate.</param>
    /// <param name="context">The validation context.</param>
    /// <returns>A collection of validation results.</returns>
    public IEnumerable<ValidationResult> Validate(List<object> values, ValidationContext context)
    {
        if (values.Any(x => x as string == null))
        {
            return new[] { new ValidationResult($"{nameof(values)} are no {nameof(IEnumerable)}") };
        }

        var stringValues = values.Cast<string>().ToArray();
        return ValidateConstraint(stringValues, context);
    }
}
