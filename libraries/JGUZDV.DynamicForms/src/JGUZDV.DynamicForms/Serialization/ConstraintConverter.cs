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
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException("Expected StartObject token.");
        }

        reader.Read();
        if (reader.TokenType != JsonTokenType.PropertyName || reader.GetString() != "$Type")
        {
            throw new JsonException("Expected $Type property.");
        }

        reader.Read();
        if (reader.TokenType != JsonTokenType.String) {
            throw new JsonException("Expected string value for $Type property.");
        }

        var constraintId = new ConstraintId(reader.GetString()!);

        reader.Read();
        if (reader.TokenType != JsonTokenType.PropertyName || reader.GetString() != "$Value")
        {
            throw new JsonException("Expected $Value property.");
        }

        reader.Read();
        var valueElement = JsonElement.ParseValue(ref reader);
        if (!DynamicFormsConfiguration.RegisteredConstraintTypes.TryGetValue(constraintId, out var constraintType))
        {
            throw new JsonException($"Unknown constraint type: {constraintId}");
        }

        if (reader.TokenType != JsonTokenType.EndObject) {
            throw new JsonException("Expected EndObject token.");
        }

        reader.Read();

        return (IConstraint?)JsonSerializer.Deserialize(valueElement, constraintType, options);
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, IConstraint value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WriteString("$Type", value.GetConstraintId().Value);

        writer.WritePropertyName("$Value");
        JsonSerializer.Serialize(writer, value, value.GetType(), options);

        writer.WriteEndObject();
    }
}
