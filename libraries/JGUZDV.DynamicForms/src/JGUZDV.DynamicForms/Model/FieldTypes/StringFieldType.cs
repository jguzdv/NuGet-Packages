using System.Text.RegularExpressions;

using JGUZDV.DynamicForms.Model.Constraints;
using JGUZDV.L10n;

namespace JGUZDV.DynamicForms.Model.FieldTypes;

/// <summary>
/// Represents a field type for string values.
/// </summary>
public class StringFieldType : FieldType
{
    private StringFieldType() : base(
        typeof(string),
        new L10nString()
        {
            ["de"] = "Text",
            ["en"] = "Text"
        },
        [
            RangeConstraint.ConstraintId,
            RegexConstraint.ConstraintId,
            StringLengthConstraint.ConstraintId
        ])
    { }

    /// <summary>
    /// Gets the singleton instance of the <see cref="StringFieldType"/>.
    /// </summary>
    public static StringFieldType Instance { get; } = new StringFieldType();


    /// <inheritdoc/>
    public override string ConvertFromValue(object value) 
        => (string)value;

    /// <inheritdoc/>
    public override object ConvertToValue(string stringValue)
        => stringValue;
}
