using System.Text.Json;
using System.Text.Json.Serialization;

using JGUZDV.DynamicForms.Model;

namespace JGUZDV.DynamicForms.Serialization;

/// <summary>
/// JSONConverter for <see cref="FieldId"/>
/// </summary>
public class FieldIdConverter : JsonConverter<FieldId>
{
    /// <inheritdoc/>
    public override FieldId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        return !string.IsNullOrWhiteSpace(value) 
            ? new(value) 
            : throw new JsonException("Could not parse value as FieldId");
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, FieldId value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Value);
    }
}
