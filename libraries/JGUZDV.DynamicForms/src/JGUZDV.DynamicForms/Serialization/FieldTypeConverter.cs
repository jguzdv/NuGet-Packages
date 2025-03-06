using System.Text.Json;
using System.Text.Json.Serialization;

using JGUZDV.DynamicForms.Model;

namespace JGUZDV.DynamicForms.Serialization;

/// <inheritdoc />
public class FieldTypeConverter : JsonConverter<FieldType>
{
    /// <inheritdoc />
    public override FieldType? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
        {
            if (doc.RootElement.TryGetProperty("$Type", out JsonElement typeElement))
            {
                string typeName = typeElement.GetString()!;
                Type? fieldType = Type.GetType(typeName);

                if (fieldType == null || !typeof(FieldType).IsAssignableFrom(fieldType))
                {
                    throw new InvalidOperationException("Unable to determine the type of the field type.");
                }

                return (FieldType?)JsonSerializer.Deserialize(doc.RootElement.GetProperty("$Value"), fieldType, options);
            }
        }

        throw new JsonException("Unable to determine the type of the field type.");
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, FieldType value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WriteString("$Type", value.GetType().FullName);

        writer.WritePropertyName("$Value");
        JsonSerializer.Serialize(writer, value, value.GetType(), options);

        writer.WriteEndObject();
    }
}
