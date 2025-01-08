using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Text.RegularExpressions;
namespace JGUZDV.DynamicForms.Model;


public abstract class Constraint : IValidatableObject
{
    public abstract IEnumerable<ValidationResult> ValidateConstraint(List<object> values, ValidationContext context);

    public abstract IEnumerable<ValidationResult> Validate(ValidationContext validationContext);

}


public class RegexConstraint : Constraint
{
    public string Regex { get; set; } = "";

    public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrEmpty(Regex))
        {
            yield return new ValidationResult($"{nameof(Regex)} must be set");
        }
    }

    public IEnumerable<ValidationResult> ValidateConstraint(string[] values, ValidationContext context)
    {
        var regex = new Regex(Regex);

        var fields = new List<string?> { (context.ObjectInstance as FieldDefinition)?.InputDefinition.Name }.Where(x => x != null).ToList();

        return values.Where(value => !regex.IsMatch(value)).Select(value => new ValidationResult("Constraint.Validation.Regex", fields!));
    }

    public override IEnumerable<ValidationResult> ValidateConstraint(List<object> values, ValidationContext context)
    {
        if (values.Any(x => x as string == null))
        {
            return [new ValidationResult($"{nameof(values)} are no strings")];
        }

        var stringValues = values.Cast<string>().ToArray();
        return ValidateConstraint(stringValues, context);
    }
}

public class RangeConstraint : Constraint
{
    private IComparable? _maxValue;
    private IComparable? _minValue;

    public IComparable? MaxValue
    {
        get => _maxValue;
        set
        {
            _maxValue = value;
        }
    }
    public IComparable? MinValue
    {
        get => _minValue;
        set
        {
            _minValue = value;
        }
    }

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

public class SizeConstraint : Constraint
{
    public int MinCount { get; set; }
    public int MaxCount { get; set; }

    public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (MinCount > MaxCount)
        {
            yield return new ValidationResult($"{nameof(MinCount)} must be less than {nameof(MaxCount)}", new[] { nameof(MinCount), nameof(MaxCount) });
        }
    }

    public IEnumerable<ValidationResult> ValidateConstraint(IList values, ValidationContext context)
    {
        var fields = new List<string?> { (context.ObjectInstance as FieldDefinition)?.InputDefinition.Name }.Where(x => x != null).ToList();

        if (!(values.Count >= MinCount && values.Count <= MaxCount))
            yield return new ValidationResult("Constraint.Validation.Size", fields!);
    }

    public override IEnumerable<ValidationResult> ValidateConstraint(List<object> values, ValidationContext context)
    {
        return ValidateConstraint(values, context);
    }
}

public class StringLengthConstraint : Constraint
{
    public int MaxLength { get; set; }

    public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (MaxLength < 0)
        {
            yield return new ValidationResult($"{nameof(MaxLength)} may not be negative");
        }
    }

    public IEnumerable<ValidationResult> ValidateConstraint(string[] values, ValidationContext context)
    {
        var fields = new List<string?> { (context.ObjectInstance as FieldDefinition)?.InputDefinition.Name }.Where(x => x != null).ToList();

        return values.Where(value => value.Count() > MaxLength).Select(x => new ValidationResult("Constraint.Validation.Length", fields!));
    }

    public override IEnumerable<ValidationResult> ValidateConstraint(List<object> values, ValidationContext context)
    {
        if (values.Any(x => x as string == null))
        {
            return [new ValidationResult($"{nameof(values)} are no {nameof(IEnumerable)}")];
        }

        var stringValues = values.Cast<string>().ToArray();
        return ValidateConstraint(stringValues, context);
    }
}