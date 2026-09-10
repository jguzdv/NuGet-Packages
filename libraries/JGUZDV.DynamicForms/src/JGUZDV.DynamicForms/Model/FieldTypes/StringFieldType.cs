using System.Text.Json.Serialization;

using JGUZDV.L10n;

namespace JGUZDV.DynamicForms.Model.FieldTypes;

/// <summary>
/// Represents a field type for string values.
/// </summary>
public record StringFieldType : FieldType
{
    /// <summary>
    /// Gets the type discriminator for the <see cref="StringFieldType"/>.
    /// </summary>
    public static FieldTypeId FieldTypeId { get; } = new("String");

    /// <inheritdoc/>
    public override FieldTypeId TypeDiscriminator => FieldTypeId;

    /// <inheritdoc/>
    public override Type ClrType => typeof(string);

    /// <inheritdoc/>
    public override string ConvertFromValue(object value)
    {
        return (string)value;
    }

    /// <inheritdoc/>
    public override object ConvertToValue(string stringValue)
    {
        return stringValue;
    }

    /// <inheritdoc/>
    public override L10nString DisplayName => new L10nString()
    {
        ["de"] = "Text",
        ["en"] = "Text"
    };
}
