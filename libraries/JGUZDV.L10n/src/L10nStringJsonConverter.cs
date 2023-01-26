using System.Text.Json;
using System.Text.Json.Serialization;

namespace ZDV.L10n
{
    public class L10nStringJsonConverter : JsonConverter<L10nString>
    {
        public override L10nString? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
                throw new JsonException();

            var result = new L10nString();

            reader.Read();
            while (reader.TokenType != JsonTokenType.EndObject)
            {
                if (reader.TokenType != JsonTokenType.PropertyName)
                    throw new JsonException();

                var propertyName = reader.GetString();
                if (string.IsNullOrEmpty(propertyName))
                    throw new JsonException();

                reader.Read();
                var value = reader.GetString();
                if (!string.IsNullOrWhiteSpace(value))
                    result[propertyName] = value;

                reader.Read();
            }

            return result;
        }

        public override void Write(Utf8JsonWriter writer, L10nString value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            foreach(var l10nValue in value.Values)
            {
                writer.WriteString(l10nValue.Key, l10nValue.Value);
            }
            writer.WriteEndObject();
        }
    }
}