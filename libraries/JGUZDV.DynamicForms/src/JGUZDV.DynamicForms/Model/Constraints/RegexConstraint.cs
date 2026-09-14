using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Text.RegularExpressions;

using JGUZDV.L10n;

namespace JGUZDV.DynamicForms.Model.Constraints;

/// <summary>
/// Constraint for validating values against a regular expression.
/// </summary>
public class RegexConstraint : IConstraint
{
    /// <inheritdoc />
    public static ConstraintId ConstraintId => new(nameof(RegexConstraint));

    /// <inheritdoc />
    public ConstraintId GetConstraintId() => ConstraintId;

    /// <inheritdoc />
    public static L10nString DisplayName => new() { ["de"] = "Regulärer Ausdruck", ["en"] = "Regular Expression" };

    /// <inheritdoc />
    public L10nString GetDisplayName() => DisplayName;



    /// <summary>
    /// The regular expression pattern.
    /// </summary>
    public string Regex { get; set; } = "";

    /// <summary>
    /// Validates the constraint.
    /// </summary>
    /// <param name="validationContext">The validation context.</param>
    /// <returns>A collection of validation results.</returns>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrEmpty(Regex))
        {
            yield return new ValidationResult($"{nameof(Regex)} must be set");
        }
    }

    /// <summary>
    /// Validates the values against the regular expression constraint.
    /// </summary>
    /// <param name="values">The values to validate.</param>
    /// <param name="context">The validation context.</param>
    /// <returns>A collection of validation results.</returns>
    public IEnumerable<ValidationResult> ValidateConstraint(string[] values, ValidationContext context)
    {
        var regex = new Regex(Regex);

        var fields = new List<string?> { (context.ObjectInstance as FieldDefinition)?.InputDefinition.Name }.Where(x => x != null).ToList();

        return values.Where(value => !regex.IsMatch(value)).Select(value => new ValidationResult("Constraint.Validation.Regex", fields!));
    }

    /// <summary>
    /// Validates the values against the regular expression constraint.
    /// </summary>
    /// <param name="values">The values to validate.</param>
    /// <param name="context">The validation context.</param>
    /// <returns>A collection of validation results.</returns>
    public IEnumerable<ValidationResult> Validate(List<object> values, ValidationContext context)
    {
        if (values.Any(x => x as string == null))
        {
            return new[] { new ValidationResult($"{nameof(values)} are no strings") };
        }

        var stringValues = values.Cast<string>().ToArray();
        return ValidateConstraint(stringValues, context);
    }
}
