using System.Text.Json.Serialization;

using JGUZDV.L10n;

namespace JGUZDV.DynamicForms.Model.FieldTypes;

/// <summary>
/// Represents a field type for integer values.
/// </summary>
public record IntFieldType : FieldType
{
    /// <summary>
    /// Gets the type discriminator for the <see cref="IntFieldType"/>.
    /// </summary>
    public static FieldTypeId FieldTypeId { get; } = new("Int32");

    /// <inheritdoc/>
    public override FieldTypeId TypeDiscriminator => FieldTypeId;

    /// <inheritdoc/>
    public override Type ClrType => typeof(long);

    /// <inheritdoc/>
    public override L10nString DisplayName => new L10nString()
    {
        ["de"] = "Ganzzahl",
        ["en"] = "Integer"
    };

    /// <inheritdoc/>
    public override string HtmlInputType => "number";
}
