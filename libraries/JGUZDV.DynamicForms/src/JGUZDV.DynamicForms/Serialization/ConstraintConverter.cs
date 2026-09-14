using System.Text.Json;
using System.Text.Json.Serialization;

using JGUZDV.DynamicForms.Model.Constraints;

namespace JGUZDV.DynamicForms.Serialization;

/// <summary>
/// A custom JSON converter for the <see cref="IConstraint"/> interface.
/// </summary>
public class ConstraintConverter : JsonConverter<IConstraint>
{
    /// <inheritdoc />
    public override IConstraint? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
        {
            if (doc.RootElement.TryGetProperty("$Type", out JsonElement typeElement))
            {
                string typeName = typeElement.GetString()!;
                Type? constraintType = Type.GetType(typeName);

                if (constraintType == null || !typeof(IConstraint).IsAssignableFrom(constraintType))
                {
                    throw new InvalidOperationException("Unable to determine the type of the constraint.");
                }

                return (IConstraint?)JsonSerializer.Deserialize(doc.RootElement.GetProperty("$Value"), constraintType, options);
            }
        }

        throw new JsonException("Unable to determine the type of the constraint.");
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, IConstraint value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WriteString("$Type", value.GetType().FullName);

        writer.WritePropertyName("$Value");
        JsonSerializer.Serialize(writer, value, value.GetType(), options);

        writer.WriteEndObject();
    }
}
