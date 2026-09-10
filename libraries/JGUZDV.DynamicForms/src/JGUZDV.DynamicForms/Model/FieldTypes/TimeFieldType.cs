using System.Text.Json;

using JGUZDV.DynamicForms.Model.FieldTypes;
using JGUZDV.L10n;

namespace JGUZDV.DynamicForms.Model;

/// <summary>
/// Represents a field type for TimeOnly values.
/// </summary>
public record TimeOnlyFieldType : FieldType
{
    /// <summary>
    /// Gets the type discriminator for the <see cref="TimeOnlyFieldType"/>.
    /// </summary>
    public static FieldTypeId FieldTypeId { get; } = new("TimeOnly");

    /// <inheritdoc/>
    public override FieldTypeId TypeDiscriminator => FieldTypeId;

    /// <inheritdoc/>
    public override Type ClrType => typeof(TimeOnly);

    /// <inheritdoc/>
    public override L10nString DisplayName => new L10nString()
    {
        ["de"] = "Uhrzeit",
        ["en"] = "Time"
    };

    /// <inheritdoc/>
    public override string HtmlInputType => "time";

    /// <inheritdoc/>
    public override string ConvertFromValue(object value)
    {
        if (value is TimeOnly timeOnly)
        {
            return timeOnly.ToString("O");
        }
        throw new InvalidOperationException($"Invalid value type: {value.GetType().Name}. Expected TimeOnly.");
    }

    /// <inheritdoc/>
    public override object ConvertToValue(string stringValue)
    {
        if (TimeOnly.TryParse(stringValue, out var dateOnly))
        {
            return dateOnly;
        }
        else
        {
            return JsonSerializer.Deserialize<TimeOnly>(stringValue, DynamicFormsConfiguration.JsonSerializerOptions);
        }
    }
}
