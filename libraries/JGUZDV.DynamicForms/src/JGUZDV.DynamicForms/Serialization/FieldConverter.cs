using System.Text.Json;
using System.Text.Json.Serialization;

using JGUZDV.DynamicForms.Model;

public class FieldConverter : JsonConverter<Field>
{
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

                    var fieldType = FieldType.FromJson(fieldDefinition.InputDefinition.Type);
                    value = fieldType.ConvertToValue(reader.GetString()!);
                    break;
            }
        }

        if (fieldDefinition == null)
            throw new JsonException("FieldDefinition is required");

        return new Field(fieldDefinition, value);
    }

    public override void Write(Utf8JsonWriter writer, Field value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WritePropertyName(nameof(Field.FieldDefinition));
        JsonSerializer.Serialize(writer, value.FieldDefinition, options);

        writer.WritePropertyName(nameof(Field.Value));
        var fieldType = FieldType.FromJson(value.FieldDefinition.InputDefinition.Type);
        writer.WriteStringValue(fieldType.ConvertFromValue(value.Value!));

        writer.WriteEndObject();
    }
}
