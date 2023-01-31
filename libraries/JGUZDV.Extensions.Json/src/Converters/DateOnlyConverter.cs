#if NET6_0
using System.Text.Json;
using System.Text.Json.Serialization;

namespace JGUZDV.Extensions.Json.Converters;

public class DateOnlyConverter : JsonConverter<DateOnly>
{
    public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidOperationException("DateOnly can't be null");

        return DateOnly.ParseExact(value, "yyyy'-'MM'-'dd");
    }

    public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString("yyyy'-'MM'-'dd"));
    }
}
#endif