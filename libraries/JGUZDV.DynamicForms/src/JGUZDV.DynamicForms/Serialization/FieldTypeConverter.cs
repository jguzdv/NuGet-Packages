using System.Text.Json;
using System.Text.Json.Serialization;

using JGUZDV.DynamicForms.Model.FieldTypes;

namespace JGUZDV.DynamicForms.Serialization;

/// <inheritdoc />
public class FieldTypeConverter : JsonConverter<FieldType>
{
    /// <inheritdoc />
    public override FieldType? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var typeDiscriminator = reader.GetString() 
            ?? throw new JsonException("Unable to determine the type of the field type.");

        return DynamicFormsConfiguration.RegisteredFieldTypes.TryGetValue(new(typeDiscriminator), out var fieldType)
            ? fieldType
            : throw new JsonException($"Unknown field type discriminator: {typeDiscriminator}");
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, FieldType value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.TypeDiscriminator.Value);
    }
}
