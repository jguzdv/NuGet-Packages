using System.Text.Json.Serialization;

using JGUZDV.DynamicForms.Model.Constraints;
using JGUZDV.L10n;

namespace JGUZDV.DynamicForms.Model.FieldTypes;

/// <summary>
/// Represents a field type for integer values.
/// </summary>
public class IntFieldType : BaseFieldType
{
    private IntFieldType() : base(
        typeof(int),
        new L10nString()
        {
            ["de"] = "Ganzzahl",
            ["en"] = "Integer"
        },
        [RangeConstraint.ConstraintId])
    {
        HtmlInputType = "number";
    }

    /// <summary>
    /// Gets the singleton instance of the <see cref="IntFieldType"/>.
    /// </summary>
    public static IntFieldType Instance { get; } = new IntFieldType();
}
