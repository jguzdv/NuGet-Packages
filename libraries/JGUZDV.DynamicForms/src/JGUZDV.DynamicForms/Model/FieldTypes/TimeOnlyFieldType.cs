using System.Text.Json;

using JGUZDV.DynamicForms.Model.Constraints;
using JGUZDV.L10n;

namespace JGUZDV.DynamicForms.Model.FieldTypes;

/// <summary>
/// Represents a field type for TimeOnly values.
/// </summary>
public class TimeOnlyFieldType : FieldType
{
    private TimeOnlyFieldType() : base(
        typeof(TimeOnly),
        new L10nString()
        {
            ["de"] = "Uhrzeit",
            ["en"] = "Time"
        },
        [RangeConstraint.ConstraintId])
    {
        HtmlInputType = "time";
    }

    /// <summary>
    /// Gets the singleton instance of the <see cref="TimeOnlyFieldType"/>.
    /// </summary>
    public static TimeOnlyFieldType Instance { get; } = new TimeOnlyFieldType();


    /// <inheritdoc/>
    public override string ConvertFromValue(object value)
    {
        return value is TimeOnly timeOnly
            ? timeOnly.ToString("O")
            : throw new InvalidOperationException($"Invalid value type: {value.GetType().Name}. Expected TimeOnly.");
    }

    /// <inheritdoc/>
    public override object ConvertToValue(string stringValue)
    {
        return TimeOnly.TryParse(stringValue, out var timeOnly)
            ? timeOnly
            : JsonSerializer.Deserialize<TimeOnly>(stringValue, DynamicFormsConfiguration.JsonSerializerOptions);
    }
}
