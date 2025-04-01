using System.Text.Json;

using JGUZDV.DynamicForms.Model;

using Microsoft.AspNetCore.Http;

namespace JGUZDV.DynamicForms.Extensions.Models;

/// <summary>
/// Represents a form field.
/// </summary>
public class FormField
{
    /// <summary>
    /// Gets or sets the field identifier.
    /// </summary>
    public required string FieldIdentifier { get; set; }

    /// <summary>
    /// The value of the <see cref="Field"/> as json
    /// </summary>
    public required string Json { get; set; }


    /// <summary>
    /// Deserializes the json value to a field value.
    /// </summary>
    /// <param name="definition"></param>
    /// <returns></returns>
    public object? ToFieldValue(FieldDefinition definition)
    {
        var type = definition.IsList
            ? typeof(List<>).MakeGenericType(definition.Type!.ClrType)
        : definition.Type!.ClrType;

        return JsonSerializer.Deserialize(Json, type);
    }
}
