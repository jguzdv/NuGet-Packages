using System.Text.Json;
using System.Text.Json.Serialization;

namespace JGUZDV.Extensions.Json.Converters;

public class StringTrimmingJsonConverter : JsonConverter<string>
{
    private readonly bool _trimToNull;

    public StringTrimmingJsonConverter(bool trimToNull = true)
    {
        _trimToNull = trimToNull;
    }

    public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        return Trim(value);
    }

    public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(Trim(value));
    }

    private string? Trim(string? input)
    {
        if (!_trimToNull)
            return input?.Trim();

        var result = input?.Trim();
        return string.IsNullOrEmpty(result) ? null : result;
    }
}
