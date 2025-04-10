using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

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
