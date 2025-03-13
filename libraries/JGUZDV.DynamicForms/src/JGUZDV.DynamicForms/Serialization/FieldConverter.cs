using System.Text.Json;
using System.Text.Json.Serialization;

using JGUZDV.DynamicForms.Model;

/// <inheritdoc />
public class FieldConverter : JsonConverter<Field>
{
    /// <inheritdoc />
    public override Field? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException();

        FieldDefinition? fieldDefinition = null;
        object? value = null;

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
                break;

            if (reader.TokenType != JsonTokenType.PropertyName)
                throw new JsonException();

            string propertyName = reader.GetString()!;
            reader.Read();

            switch (propertyName)
            {
                case nameof(Field.FieldDefinition):
                    fieldDefinition = JsonSerializer.Deserialize<FieldDefinition>(ref reader, options);
                    break;
                case nameof(Field.Value):
                    if (fieldDefinition == null)
                        throw new JsonException("FieldDefinition must be read before Value");

                    var fieldType = fieldDefinition.Type ?? throw new InvalidOperationException("FieldType must be set");

                    var jsonValue = reader.GetString();
                    value = jsonValue != null
                        ? fieldType.ConvertToValue(jsonValue)
                        : null;
                    break;
            }
        }

        if (fieldDefinition == null)
            throw new JsonException("FieldDefinition is required");

        return new Field(fieldDefinition, value);
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, Field value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WritePropertyName(nameof(Field.FieldDefinition));
        JsonSerializer.Serialize(writer, value.FieldDefinition, options);

        writer.WritePropertyName(nameof(Field.Value));
        var fieldType = value.FieldDefinition.Type ?? throw new InvalidOperationException("FieldType must be set");

        if (value.Value == null)
        {
            writer.WriteNullValue();
        }
        else
        {
            writer.WriteStringValue(fieldType.ConvertFromValue(value.Value!));
        }

        writer.WriteEndObject();
    }
}
