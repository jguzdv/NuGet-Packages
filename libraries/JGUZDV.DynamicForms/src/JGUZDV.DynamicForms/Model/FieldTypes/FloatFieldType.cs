using System.Text.Json.Serialization;

using JGUZDV.DynamicForms.Model.FieldTypes;
using JGUZDV.L10n;

namespace JGUZDV.DynamicForms.Model;

/// <summary>
/// Represents a field type for float values.
/// </summary>
public record FloatFieldType : FieldType
{
    /// <inheritdoc/>
    public override string TypeDiscriminator => "Float";

    /// <inheritdoc/>
    public override Type ClrType => typeof(float);

    /// <inheritdoc/>
    public override L10nString DisplayName => new L10nString()
    {
        ["de"] = "Gleitkommazahl",
        ["en"] = "Float" 
    };

    /// <summary>
    /// Gets the input type of the field.
    /// </summary>
    [JsonIgnore]
    public override string HtmlInputType => "number";
}
