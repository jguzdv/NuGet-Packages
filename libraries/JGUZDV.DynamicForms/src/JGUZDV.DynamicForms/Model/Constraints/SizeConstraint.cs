using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Data;

using JGUZDV.L10n;

namespace JGUZDV.DynamicForms.Model.Constraints;

/// <summary>
/// Constraint for validating the size of a collection.
/// </summary>
public class SizeConstraint : IConstraint
{
    /// <inheritdoc />
    public static ConstraintId ConstraintId => new(nameof(SizeConstraint));
    
    /// <inheritdoc />
    public ConstraintId GetConstraintId() => ConstraintId;
    
    /// <inheritdoc />
    public static L10nString DisplayName => new() { ["de"] = "Listenlänge", ["en"] = "List length" };




    /// <summary>
    /// The minimum count of the collection.
    /// </summary>
    public int MinCount { get; set; }
    /// <summary>
    /// The maximum count of the collection.
    /// </summary>
    public int MaxCount { get; set; }

    /// <summary>
    /// Validates the constraint.
    /// </summary>
    /// <param name="validationContext">The validation context.</param>
    /// <returns>A collection of validation results.</returns>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (MinCount > MaxCount)
        {
            yield return new ValidationResult($"{nameof(MinCount)} must be less than {nameof(MaxCount)}", new[] { nameof(MinCount), nameof(MaxCount) });
        }
    }

    /// <summary>
    /// Validates the values against the size constraint.
    /// </summary>
    /// <param name="values">The values to validate.</param>
    /// <param name="context">The validation context.</param>
    /// <returns>A collection of validation results.</returns>
    public IEnumerable<ValidationResult> ValidateConstraint(IList values, ValidationContext context)
    {
        var fields = new List<string?> { (context.ObjectInstance as FieldDefinition)?.InputDefinition.Name }.Where(x => x != null).ToList();

        if (!(values.Count >= MinCount && values.Count <= MaxCount))
            yield return new ValidationResult("Constraint.Validation.Size", fields!);
    }

    /// <summary>
    /// Validates the values against the size constraint.
    /// </summary>
    /// <param name="values">The values to validate.</param>
    /// <param name="context">The validation context.</param>
    /// <returns>A collection of validation results.</returns>
    public IEnumerable<ValidationResult> Validate(List<object> values, ValidationContext context)
    {
        return Validate(values, context);
    }
}
