using System.Text.Json.Serialization;

using JGUZDV.DynamicForms.Model.Constraints;
using JGUZDV.DynamicForms.Model.FieldTypes;
using JGUZDV.L10n;

namespace JGUZDV.DynamicForms.Model;

/// <summary>
/// Represents a field type for float values.
/// </summary>
public class FloatFieldType : BaseFieldType
{
    private FloatFieldType() : base(
        typeof(float),
        new L10nString()
        {
            ["de"] = "Gleitkommazahl",
            ["en"] = "Float"
        },
        [RangeConstraint.ConstraintId])
    {
        HtmlInputType = "number";
    }

    /// <summary>
    /// Gets the type discriminator for the <see cref="FloatFieldType"/>.
    /// </summary>
    public static FloatFieldType Instance { get; } = new FloatFieldType();
}
