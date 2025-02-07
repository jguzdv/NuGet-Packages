using System.ComponentModel.DataAnnotations;

namespace JGUZDV.DynamicForms.Model;

/// <summary>
/// Constraint for validating the size of a file.
/// </summary>
public class FileSizeConstraint : Constraint
{
    /// <summary>
    /// The maximum file size in bytes.
    /// </summary>
    public long MaxFileSize { get; set; }

    /// <summary>
    /// Validates the constraint.
    /// </summary>
    /// <param name="validationContext">The validation context.</param>
    /// <returns>A collection of validation results.</returns>
    public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        // must be postive
        if (MaxFileSize < 0)
        {
            return new[] { new ValidationResult("MaxFileSize must be a positive number.", new[] { nameof(MaxFileSize) }) };
        }

        return Enumerable.Empty<ValidationResult>();
    }

    /// <summary>
    /// Validates the values against the file size constraint.
    /// </summary>
    /// <param name="values">The values to validate.</param>
    /// <param name="context">The validation context.</param>
    /// <returns>A collection of validation results.</returns>
    public override IEnumerable<ValidationResult> ValidateConstraint(List<object> values, ValidationContext context)
    {
        var results = new List<ValidationResult>();

        foreach (var value in values)
        {
            if (value is FileFieldType.FileType file && file.FileSize > MaxFileSize)
            {
                results.Add(new ValidationResult($"File size exceeds the maximum allowed size of {MaxFileSize} bytes.", new[] { context.MemberName }));
            }
        }

        return results;
    }
}
