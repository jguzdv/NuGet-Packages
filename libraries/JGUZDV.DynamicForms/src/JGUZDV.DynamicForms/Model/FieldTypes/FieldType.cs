using System.Text.Json;
using System.Text.Json.Serialization;

using JGUZDV.DynamicForms.Serialization;
using JGUZDV.L10n;

namespace JGUZDV.DynamicForms.Model.FieldTypes;

/// <summary>
/// Represents the base class for field types.
/// </summary>
[JsonConverter(typeof(FieldTypeConverter))]
public abstract record FieldType
{
    /// <summary>
    /// Gets the CLR type of the field.
    /// </summary>
    public abstract Type ClrType { get; }

    /// <summary>
    /// Gets the display name of the field type.
    /// </summary>
    public abstract L10nString DisplayName { get; }

    /// <summary>
    /// Gets the input type of the field.
    /// </summary>
    public virtual string HtmlInputType { get; } = "text";

    /// <summary>
    /// Gets the type discriminator for the field type.
    /// This will be used to identify the field type during serialization and deserialization.
    /// </summary>
    public abstract FieldTypeId TypeDiscriminator { get; }

    /// <summary>
    /// Converts the specified value to a string.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <returns>A string representation of the value.</returns>
    public virtual string ConvertFromValue(object value)
    {
        return ClrType.IsPrimitive
            ? value?.ToString() ?? ""
            : JsonSerializer.Serialize(value, DynamicFormsConfiguration.JsonSerializerOptions);
    }

    /// <summary>
    /// Converts the specified string to an object of the CLR type.
    /// </summary>
    /// <param name="stringValue">The string to convert.</param>
    /// <returns>An object of the CLR type.</returns>
    public virtual object ConvertToValue(string stringValue)
    {
        return ClrType.IsPrimitive
            ? Convert.ChangeType(stringValue, ClrType)
            : JsonSerializer.Deserialize(stringValue, ClrType, DynamicFormsConfiguration.JsonSerializerOptions) ?? throw new InvalidOperationException($"Could not parse json: {stringValue} into target type: {ClrType.Name}");
    }

    /// <summary>
    /// Converts the field type to a JSON string.
    /// </summary>
    /// <returns>A JSON string representation of the field type.</returns>
    public string ToJson()
    {
        return System.Text.Json.JsonSerializer.Serialize(this, DynamicFormsConfiguration.JsonSerializerOptions);
    }

    /// <summary>
    /// Creates a field type from the specified JSON string.
    /// </summary>
    /// <param name="json">The JSON string to parse.</param>
    /// <returns>A field type object.</returns>
    public static FieldType FromJson(string json)
    {
        var options = new JsonSerializerOptions()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
        };
        return JsonSerializer.Deserialize<FieldType>(json, options) ?? throw new InvalidOperationException($"Could not parse json: {json}");
    }

    /// <summary>
    /// Adds the field value to the content.
    /// </summary>
    public virtual void AddToContent(Field field, MultipartFormDataContent content, string name = "")
    {
        var json = JsonSerializer.Serialize(field.Value, DynamicFormsConfiguration.JsonSerializerOptions);

        name = string.IsNullOrWhiteSpace(name)
            ? $"{DynamicFormsConfiguration.FormFieldPrefix}{field.FieldDefinition.Identifier}"
            : name;

        content.Add(new StringContent(json), name);
    }
}
