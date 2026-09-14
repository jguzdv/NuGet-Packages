using System.Text.Json;
using System.Text.Json.Serialization;

using JGUZDV.DynamicForms.Model.Constraints;
using JGUZDV.DynamicForms.Serialization;
using JGUZDV.L10n;

namespace JGUZDV.DynamicForms.Model.FieldTypes;

/// <summary>
/// Represents the base class for field types.
/// Field types define the data type and converters for form fields in the dynamic forms system.
/// These field types are meant to be used as a singleton, so make the ctor private and provide a Instance field.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="BaseFieldType"/> class with the specified CLR type, display name, and allowed constraints.
/// </remarks>
/// <param name="clrType"></param>
/// <param name="displayName"></param>
/// <param name="allowedConstraints"></param>
[JsonConverter(typeof(FieldTypeConverter))]
public abstract class BaseFieldType(
    Type clrType,
    L10nString displayName,
    HashSet<ConstraintId> allowedConstraints)
{

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseFieldType"/> class with the specified CLR type and display name.
    /// </summary>
    /// <param name="clrType"></param>
    /// <param name="displayName"></param>
    protected BaseFieldType(Type clrType, L10nString displayName)
        : this(clrType, displayName, [])
    { }

    /// <summary>
    /// Gets the CLR type of the field.
    /// </summary>
    public Type ClrType { get; } = clrType;

    /// <summary>
    /// Gets the allowed constraints for the field type.
    /// </summary>
    public HashSet<ConstraintId> AllowedConstraints { get; } = allowedConstraints;

    /// <summary>
    /// Gets the type identifier for the field type.
    /// This will be used to identify the field type during serialization and deserialization.
    /// </summary>
    public virtual FieldTypeId TypeId
    {
        get
        {
            return new(ClrType.Name);
        }
    }


    /// <summary>
    /// Gets the display name of the field type.
    /// </summary>
    public L10nString DisplayName { get; } = displayName;

    /// <summary>
    /// Gets the input type of the field.
    /// </summary>
    public string HtmlInputType { get; protected set; } = "text";


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
    public virtual object ConvertToValue(string stringValue)
    {
        return ClrType.IsPrimitive
            ? Convert.ChangeType(stringValue, ClrType)
            : JsonSerializer.Deserialize(stringValue, ClrType, DynamicFormsConfiguration.JsonSerializerOptions)
            ?? throw new InvalidOperationException($"Could not parse json: {stringValue} into target type: {ClrType.Name}");
    }

    /// <summary>
    /// Converts the specified JSON element to an object of the CLR type.
    /// </summary>
    public virtual object? ConvertToValue(JsonElement jsonElement)
        => JsonSerializer.Deserialize(jsonElement, ClrType, DynamicFormsConfiguration.JsonSerializerOptions);

    /// <summary>
    /// Converts the field type to a JSON string.
    /// </summary>
    /// <returns>A JSON string representation of the field type.</returns>
    public string ToJson()
    {
        return JsonSerializer.Serialize(this, DynamicFormsConfiguration.JsonSerializerOptions);
    }

    /// <summary>
    /// Creates a field type from the specified JSON string.
    /// </summary>
    /// <param name="json">The JSON string to parse.</param>
    /// <returns>A field type object.</returns>
    public static BaseFieldType FromJson(string json)
    {
        return JsonSerializer.Deserialize<BaseFieldType>(json, DynamicFormsConfiguration.JsonSerializerOptions) 
            ?? throw new InvalidOperationException($"Could not parse json: {json}");
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
