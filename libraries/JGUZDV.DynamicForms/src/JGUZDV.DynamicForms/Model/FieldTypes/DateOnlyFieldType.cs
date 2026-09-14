using System.Text.Json;

using JGUZDV.DynamicForms.Model.Constraints;
using JGUZDV.L10n;

namespace JGUZDV.DynamicForms.Model.FieldTypes;

/// <summary>
/// Represents a field type for DateOnly values.
/// </summary>
public class DateOnlyFieldType : BaseFieldType
{
    private DateOnlyFieldType() : base(
        typeof(DateOnly),
        new L10nString()
        {
            ["de"] = "Datum",
            ["en"] = "Date"
        },
        [RangeConstraint.ConstraintId])
    {
        HtmlInputType = "date";
    }

    /// <summary>
    /// Gets the singleton instance of the <see cref="DateOnlyFieldType"/>.
    /// </summary>
    public static DateOnlyFieldType Instance { get; } = new();

    /// <inheritdoc/>
    /// <exception cref="InvalidOperationException">Thrown when the value type is invalid.</exception>
    public override string ConvertFromValue(object value)
    {
        return value is DateOnly dateOnly
            ? dateOnly.ToString("O")
            : throw new InvalidOperationException($"Invalid value type: {value.GetType().Name}. Expected DateOnly.");
    }

    /// <inheritdoc/>
    /// <exception cref="InvalidOperationException">Thrown when the string cannot be parsed.</exception>
    public override object ConvertToValue(string stringValue)
    {
        return DateOnly.TryParse(stringValue, out var dateOnly)
            ? dateOnly
            : JsonSerializer.Deserialize<DateOnly>(stringValue, DynamicFormsConfiguration.JsonSerializerOptions);
    }
}
