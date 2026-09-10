using System.Text.Json.Serialization;

using JGUZDV.DynamicForms.Model.FieldTypes;
using JGUZDV.L10n;

namespace JGUZDV.DynamicForms.Model;

/// <summary>
/// Represents a field type for boolean values.
/// </summary>
public record BoolFieldType : FieldType
{
    /// <summary>
    /// Gets the type discriminator for the <see cref="BoolFieldType"/>.
    /// </summary>
    public static FieldTypeId FieldTypeId { get; } = new("Bool");

    /// <inheritdoc/>
    public override FieldTypeId TypeDiscriminator => FieldTypeId;

    /// <inheritdoc/>
    public override Type ClrType => typeof(bool);

    /// <inheritdoc/>
    public override L10nString DisplayName => new L10nString()
    {
        ["de"] = "Boolesch",
        ["en"] = "Boolean"
    };

    /// <inheritdoc/>
    public override string HtmlInputType => "checkbox";
}
