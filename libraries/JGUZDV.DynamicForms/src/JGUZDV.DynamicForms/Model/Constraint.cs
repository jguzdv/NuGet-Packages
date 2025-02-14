using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

using JGUZDV.DynamicForms.Serialization;
namespace JGUZDV.DynamicForms.Model;


/// <summary>
/// Base class for constraints.
/// </summary>
[JsonConverter(typeof(ConstraintConverter))]
public abstract class Constraint : IValidatableObject
{
    /// <summary>
    /// Validates <paramref name="values"/> against the constraint.
    /// </summary>
    /// <param name="values">The values to validate using the constraint.</param>
    /// <param name="context">The validation context.</param>
    /// <returns>A collection of validation results.</returns>
    public abstract IEnumerable<ValidationResult> ValidateConstraint(List<object> values, ValidationContext context);

    /// <summary>
    /// Validates the constraint.
    /// </summary>
    /// <param name="validationContext">The validation context.</param>
    /// <returns>A collection of validation results.</returns>
    public abstract IEnumerable<ValidationResult> Validate(ValidationContext validationContext);

    /// <summary>
    /// The type of the field the constraint is applied to.
    /// </summary>
    public FieldType? FieldType { get; set; }
}


/// <summary>
/// Constraint for validating values against a regular expression.
/// </summary>
public class RegexConstraint : Constraint
{
    /// <summary>
    /// The regular expression pattern.
    /// </summary>
    public string Regex { get; set; } = "";

    /// <summary>
    /// Validates the constraint.
    /// </summary>
    /// <param name="validationContext">The validation context.</param>
    /// <returns>A collection of validation results.</returns>
    public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
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
    public override IEnumerable<ValidationResult> ValidateConstraint(List<object> values, ValidationContext context)
    {
        if (values.Any(x => x as string == null))
        {
            return new[] { new ValidationResult($"{nameof(values)} are no strings") };
        }

        var stringValues = values.Cast<string>().ToArray();
        return ValidateConstraint(stringValues, context);
    }
}

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

/// <summary>
/// Constraint for validating the size of a collection.
/// </summary>
public class SizeConstraint : Constraint
{
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
    public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
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
    public override IEnumerable<ValidationResult> ValidateConstraint(List<object> values, ValidationContext context)
    {
        return ValidateConstraint(values, context);
    }
}

/// <summary>
/// Constraint for validating the length of strings.
/// </summary>
public class StringLengthConstraint : Constraint
{
    /// <summary>
    /// The maximum length of the string.
    /// </summary>
    public int MaxLength { get; set; }

    /// <summary>
    /// Validates the constraint.
    /// </summary>
    /// <param name="validationContext">The validation context.</param>
    /// <returns>A collection of validation results.</returns>
    public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
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
    public override IEnumerable<ValidationResult> ValidateConstraint(List<object> values, ValidationContext context)
    {
        if (values.Any(x => x as string == null))
        {
            return new[] { new ValidationResult($"{nameof(values)} are no {nameof(IEnumerable)}") };
        }

        var stringValues = values.Cast<string>().ToArray();
        return ValidateConstraint(stringValues, context);
    }
}
