using System.Text.Json;
using System.Text.Json.Serialization;

using JGUZDV.DynamicForms.Model;

namespace JGUZDV.DynamicForms.Serialization;

public class FieldIdConverter : JsonConverter<FieldId>
{
    public override FieldId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        return !string.IsNullOrWhiteSpace(value) 
            ? new(value) 
            : throw new JsonException("Could not parse value as FieldId");
    }

    public override void Write(Utf8JsonWriter writer, FieldId value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Value);
    }
}
