using System.Text.Json;
using System.Text.Json.Serialization;

namespace JGUZDV.DynamicForms.Serialization
{
    public class IJsonConverter<T> : JsonConverter<T>
            where T : class, IJsonConvert<T>
    {
        public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var json = reader.GetString();

            if (json == null)
                return null;

            return T.FromJson(json);
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            writer.WriteRawValue(value.ToJson());
        }
    }
}
