using System.Text.Json;
using System.Text.Json.Serialization;

using JGUZDV.L10n;

namespace JGUZDV.DynamicForms.Model.FieldTypes;

/// <summary>
/// Represents a field type for DateOnly values.
/// </summary>
public record DateOnlyFieldType : FieldType
{
    /// <inheritdoc/>
    override public string TypeDiscriminator => "DateOnly";

    /// <inheritdoc/>
    public override Type ClrType => typeof(DateOnly);

    /// <inheritdoc/>
    public override string HtmlInputType => "date";

    /// <inheritdoc/>
    public override L10nString DisplayName => new L10nString()
    {
        ["de"] = "Datum",
        ["en"] = "Date"
    };

    /// <inheritdoc/>
    /// <exception cref="InvalidOperationException">Thrown when the value type is invalid.</exception>
    public override string ConvertFromValue(object value)
    {
        if (value is DateOnly dateOnly)
        {
            return dateOnly.ToString("O");
        }
        throw new InvalidOperationException($"Invalid value type: {value.GetType().Name}. Expected DateOnly.");
    }

    /// <inheritdoc/>
    /// <exception cref="InvalidOperationException">Thrown when the string cannot be parsed.</exception>
    public override object ConvertToValue(string stringValue)
    {
        if (DateOnly.TryParse(stringValue, out var dateOnly))
        {
            return dateOnly;
        }
        else
        {
            return JsonSerializer.Deserialize<DateOnly>(stringValue, DynamicFormsConfiguration.JsonSerializerOptions);
        }
    }
}
