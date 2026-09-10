using System.Text.Json.Serialization;

using JGUZDV.DynamicForms.Serialization;

namespace JGUZDV.DynamicForms.Model;

/// <summary>
/// Represents the Id of a Field
/// </summary>
[JsonConverter(typeof(FieldIdConverter))]
public record struct FieldId(string Value)
{
    /// <summary>
    /// Represents an Empty or Invalid FieldId
    /// </summary>
    public static readonly FieldId Invalid = new ("");

    /// <summary>
    /// Returns true if, the FieldId is valid (not null or whitespace), otherwise false.
    /// </summary>
    public readonly bool IsValid => !string.IsNullOrWhiteSpace(Value);
}
