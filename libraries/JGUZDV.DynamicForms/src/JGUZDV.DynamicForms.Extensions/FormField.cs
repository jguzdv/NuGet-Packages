using JGUZDV.DynamicForms.Model;

namespace JGUZDV.DynamicForms.Extensions.Models;

/// <summary>
/// Represents a form field.
/// </summary>
public class FormField
{
    /// <summary>
    /// Gets or sets the field identifier.
    /// </summary>
    public required string FieldIdentifier { get; set; }

    /// <summary>
    /// The value of the <see cref="Field"/> as json
    /// </summary>
    public required string Json { get; set; }
}
