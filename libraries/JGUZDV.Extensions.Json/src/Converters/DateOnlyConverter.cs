#if NET6_0
using System.Text.Json;
using System.Text.Json.Serialization;

namespace JGUZDV.Extensions.Json.Converters;

/// <summary>
/// Converts a <see cref="DateOnly"/> to and from JSON.
/// </summary>
public class DateOnlyConverter : JsonConverter<DateOnly>
{
    /// <inheritdoc/>
    public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidOperationException("DateOnly can't be null");

        return DateOnly.ParseExact(value, "yyyy'-'MM'-'dd");
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString("yyyy'-'MM'-'dd"));
    }
}
#endif