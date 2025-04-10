using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Text.Json.Serialization;

using JGUZDV.DynamicForms.Serialization;
namespace JGUZDV.DynamicForms.Model;

/// <summary>
/// Constraint for validating values within a specified range.
/// </summary>
[JsonConverter(typeof(RangeConstraintConverter))]
public class RangeConstraint : Constraint
{
    private IComparable? _maxValue;
    private IComparable? _minValue;

    /// <summary>
    /// The maximum value of the range.
    /// </summary>
    public IComparable? MaxValue
    {
        get => _maxValue;
        set
        {
            _maxValue = value;
        }
    }

    /// <summary>
    /// The minimum value of the range.
    /// </summary>
    public IComparable? MinValue
    {
        get => _minValue;
        set
        {
            _minValue = value;
        }
    }

    /// <summary>
    /// Validates the values against the range constraint.
    /// </summary>
    /// <param name="values">The values to validate.</param>
    /// <param name="context">The validation context.</param>
    /// <returns>A collection of validation results.</returns>
    public override IEnumerable<ValidationResult> ValidateConstraint(List<object> values, ValidationContext context)
    {
        var fields = new List<string?> { (context.ObjectInstance as FieldDefinition)?.InputDefinition.Name }.Where(x => x != null).ToList();

        foreach (var value in values)
        {
            var isValid = true;

            if (MaxValue != null)
            {
                isValid &= MaxValue.CompareTo(value) >= 0;
            }

            if (MinValue != null)
            {
                isValid &= MinValue.CompareTo(value) <= 0;
            }

            if (!isValid)
                yield return new ValidationResult("Constraint.Validation.Range", fields!);
        }
    }

    /// <summary>
    /// Validates the constraint.
    /// </summary>
    /// <param name="validationContext">The validation context.</param>
    /// <returns>A collection of validation results.</returns>
    public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (MaxValue != null && MinValue != null)
        {
            if (MaxValue.CompareTo(MinValue) < 0)
            {
                yield return new ValidationResult($"{nameof(MaxValue)} must be greater than {nameof(MinValue)}", new[] { nameof(MaxValue), nameof(MinValue) });
            }
        }
        if (MaxValue == null && MinValue == null)
            yield return new ValidationResult("Min or Max must be set");
    }
}
