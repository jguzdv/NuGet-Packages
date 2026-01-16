using System.Text.Json;
using System.Text.Json.Serialization;

using JGUZDV.L10n;

namespace JGUZDV.DynamicForms.Model;

/// <summary>
/// Represents a field type for integer values.
/// </summary>
public record TimeFieldType : FieldType
{
    /// <summary>
    /// Gets the CLR type of the field.
    /// </summary>
    [JsonIgnore]
    public override Type ClrType => typeof(TimeOnly);

    /// <summary>
    /// Gets the display name of the field type.
    /// </summary>
    public override L10nString DisplayName => new L10nString()
    {
        ["de"] = "Uhrzeit",
        ["en"] = "Time"
    };

    /// <summary>
    /// Gets the input type of the field.
    /// </summary>
    [JsonIgnore]
    public override string InputType => "time";

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
