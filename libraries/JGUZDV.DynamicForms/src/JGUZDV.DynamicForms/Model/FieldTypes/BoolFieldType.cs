using JGUZDV.DynamicForms.Model.FieldTypes;

namespace JGUZDV.DynamicForms.Model;

/// <summary>
/// Represents a field type for boolean values.
/// </summary>
public class BoolFieldType : FieldType
{
    private BoolFieldType() : base(
        typeof(bool), 
        new()
        {
            ["de"] = "Boolesch",
            ["en"] = "Boolean"
        })
    {
        HtmlInputType = "checkbox";
    }

    /// <summary>
    /// Gets the singleton instance of the <see cref="BoolFieldType"/>.
    /// </summary>
    public static BoolFieldType Instance { get; } = new();
}
