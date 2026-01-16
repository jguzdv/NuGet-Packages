using System.Text.Json.Serialization;

using JGUZDV.L10n;

namespace JGUZDV.DynamicForms.Model;

/// <summary>
/// Represents a field type for float values.
/// </summary>
public record FloatFieldType : FieldType
{
    /// <summary>
    /// Gets the CLR type of the field.
    /// </summary>
    [JsonIgnore]
    public override Type ClrType => typeof(decimal);

    /// <summary>
    /// Gets the display name of the field type.
    /// </summary>
    public override L10nString DisplayName => new L10nString()
    {
        ["de"] = "Gleitkommazahl",
        ["en"] = "Float" 
    };

    /// <summary>
    /// Gets the input type of the field.
    /// </summary>
    [JsonIgnore]
    public override string InputType => "number";
}
