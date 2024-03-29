#if NET6_0
using System.Text.Json;
using System.Text.Json.Serialization;

namespace JGUZDV.Extensions.Json.Converters;

/// <summary>
/// Converts a <see cref="TimeOnly"/> to and from JSON.
/// </summary>
public class TimeOnlyConverter : JsonConverter<TimeOnly>
{
    /// <inheritdoc/>
    public override TimeOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidOperationException("TimeOnly can't be null");

        return TimeOnly.ParseExact(value, "HH:mm:ss");
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, TimeOnly value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString("HH:mm:ss"));
    }
}
#endif
