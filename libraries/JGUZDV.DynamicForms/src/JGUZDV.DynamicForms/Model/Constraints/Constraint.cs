using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

using JGUZDV.DynamicForms.Serialization;
using JGUZDV.L10n;

namespace JGUZDV.DynamicForms.Model.Constraints;


/// <summary>
/// Base class for constraints.
/// </summary>
[JsonConverter(typeof(ConstraintConverter))]
public interface IConstraint : IValidatableObject
{
    /// <summary>
    /// Validates <paramref name="values"/> against the constraint.
    /// </summary>
    /// <param name="values">The values to validate using the constraint.</param>
    /// <param name="context">The validation context.</param>
    /// <returns>A collection of validation results.</returns>
    IEnumerable<ValidationResult> Validate(List<object> values, ValidationContext context);

    /// <summary>
    /// Returns the unique identifier of the constraint. This will be used as discriminator during serialization and deserialization of constraints.
    /// </summary>
    /// <returns></returns>
    ConstraintId GetConstraintId();

    /// <summary>
    /// Returns the display name of the constraint. This will be used for localization and user-friendly representation of the constraint.
    /// </summary>
    L10nString GetDisplayName();


    /// <summary>
    /// Gets the unique identifier of the constraint. This will be used as discriminator during serialization and deserialization of constraints.
    /// </summary>
    virtual static ConstraintId ConstraintId { get; } = new();

    /// <summary>
    /// Gets the display name of the constraint. This will be used for localization and user-friendly representation of the constraint.
    /// </summary>
    virtual static L10nString DisplayName { get; } = new();
}
