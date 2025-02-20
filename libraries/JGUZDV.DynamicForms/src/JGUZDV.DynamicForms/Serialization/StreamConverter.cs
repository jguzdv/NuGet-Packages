using System.Text.Json;
using System.Text.Json.Serialization;

namespace JGUZDV.DynamicForms.Serialization;

internal class StreamConverter : JsonConverter<Stream>
{

    public override Stream? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            var base64String = reader.GetString();
            if (base64String != null)
            {
                var bytes = Convert.FromBase64String(base64String);
                return new MemoryStream(bytes);
            }
        }
        return null;
    }

    public override void Write(Utf8JsonWriter writer, Stream value, JsonSerializerOptions options)
    {
        if (value is MemoryStream memoryStream)
        {
            var bytes = memoryStream.ToArray();
            var base64String = Convert.ToBase64String(bytes);
            writer.WriteStringValue(base64String);
        }
        else
        {
            using (memoryStream = new MemoryStream())
            {
                value.CopyTo(memoryStream);
                var bytes = memoryStream.ToArray();
                var base64String = Convert.ToBase64String(bytes);
                writer.WriteStringValue(base64String);
            }
        }
    }
}
