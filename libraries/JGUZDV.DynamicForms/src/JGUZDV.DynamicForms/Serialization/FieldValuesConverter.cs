using System.Text.Json;
using System.Text.Json.Serialization;

using JGUZDV.DynamicForms.Model;

namespace JGUZDV.DynamicForms.Serialization;

/// <summary>
/// Represents a JSON converter for the <see cref="FieldValues"/> class, enabling serialization and deserialization of field values to and from JSON format.
/// </summary>
public class FieldValuesConverter : JsonConverter<FieldValues>
{
    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, FieldValues fieldValues, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("Values");

        writer.WriteStartArray();
        foreach (var value in fieldValues.Values)
        {
            writer.WriteStartObject();
            writer.WritePropertyName(value.Key.Value);
            writer.WriteRawValue(JsonSerializer.SerializeToUtf8Bytes(value.Value, options));
            writer.WriteEndObject();
        }
        writer.WriteEndArray();

        writer.WriteEndObject();
    }

    /// <inheritdoc />
    public override FieldValues? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException("Expected start of object.");
        }

        reader.Read();
        if (reader.TokenType != JsonTokenType.PropertyName || reader.GetString() != "Values")
        {
            throw new JsonException("Expected property 'Values'.");
        }

        reader.Read();
        if (reader.TokenType != JsonTokenType.StartArray)
        {
            throw new JsonException("Expected start of array.");
        }

        var result = new Dictionary<FieldId, object?>();

        reader.Read();
        while (reader.TokenType != JsonTokenType.EndArray)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException("Expected start of object.");
            }

            reader.Read();
            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonException("Expected property name.");
            }

            var fieldId = new FieldId(reader.GetString()!);

            reader.Read();
            var value = JsonSerializer.Deserialize<JsonElement>(ref reader, options);

            
            result[fieldId] = value;

            reader.Read();
            if (reader.TokenType != JsonTokenType.EndObject)
            {
                throw new JsonException("Expected end of object.");
            }

            reader.Read();
        }

        return new FieldValues(result);
    }
}
