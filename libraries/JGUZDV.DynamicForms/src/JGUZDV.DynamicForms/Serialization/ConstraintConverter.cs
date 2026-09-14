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

        if (reader.TokenType != JsonTokenType.String) {
            throw new JsonException("Expected string value for $Type property.");
        }

        var constraintId = new ConstraintId(reader.GetString()!);

        if (reader.TokenType != JsonTokenType.PropertyName || reader.GetString() != "$Value")
        {
            throw new JsonException("Expected $Value property.");
        }

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

        //using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
        //{
        //    if (doc.RootElement.TryGetProperty("$Type", out JsonElement typeElement))
        //    {
        //        string typeName = typeElement.GetString()!;
        //        Type? constraintType = Type.GetType(typeName);

        //        if (constraintType == null || !typeof(IConstraint).IsAssignableFrom(constraintType))
        //        {
        //            throw new InvalidOperationException("Unable to determine the type of the constraint.");
        //        }

        //        return (IConstraint?)JsonSerializer.Deserialize(doc.RootElement.GetProperty("$Value"), constraintType, options);
        //    }
        //}

        //throw new JsonException("Unable to determine the type of the constraint.");
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
