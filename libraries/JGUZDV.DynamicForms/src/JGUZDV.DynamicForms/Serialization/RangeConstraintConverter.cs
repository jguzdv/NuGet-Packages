using System.Text.Json;
using System.Text.Json.Serialization;

using JGUZDV.DynamicForms.Model;

namespace JGUZDV.DynamicForms.Serialization;

/// <inheritdoc />
public class RangeConstraintConverter : JsonConverter<RangeConstraint>
{
    /// <inheritdoc />
    public override RangeConstraint Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException();

        var rangeConstraint = new RangeConstraint();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
                return rangeConstraint;

            if (reader.TokenType != JsonTokenType.PropertyName)
                throw new JsonException();

            string propertyName = reader.GetString()!;
            reader.Read();

            switch (propertyName)
            {
                case nameof(RangeConstraint.FieldType):
                    string fieldTypeString = reader.GetString()!;
                    rangeConstraint.FieldType = FieldType.FromJson(fieldTypeString);
                    break;
                case nameof(RangeConstraint.MaxValue):
                    string maxValueString = reader.GetString()!;
                    rangeConstraint.MaxValue = rangeConstraint.FieldType!.ConvertToValue(maxValueString) as IComparable;
                    break;
                case nameof(RangeConstraint.MinValue):
                    string minValueString = reader.GetString()!;
                    rangeConstraint.MinValue = rangeConstraint.FieldType!.ConvertToValue(minValueString) as IComparable;
                    break;
                default:
                    throw new JsonException($"Unknown property: {propertyName}");
            }
        }

        return rangeConstraint;
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, RangeConstraint value, JsonSerializerOptions options)
    {
        if (value.FieldType == null)
            throw new InvalidOperationException("FieldType must be set");

        writer.WriteStartObject();

        writer.WritePropertyName(nameof(RangeConstraint.FieldType));
        writer.WriteStringValue(value.FieldType.ToJson());

        if (value.MaxValue != null)
        {
            writer.WritePropertyName(nameof(RangeConstraint.MaxValue));
            writer.WriteStringValue(value.FieldType.ConvertFromValue(value.MaxValue));
        }

        if (value.MinValue != null)
        {
            writer.WritePropertyName(nameof(RangeConstraint.MinValue));
            writer.WriteStringValue(value.FieldType.ConvertFromValue(value.MinValue));
        }

        writer.WriteEndObject();
    }

}
