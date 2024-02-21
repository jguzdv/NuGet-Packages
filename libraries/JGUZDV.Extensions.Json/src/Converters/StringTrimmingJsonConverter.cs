using System.Text.Json;
using System.Text.Json.Serialization;

namespace JGUZDV.Extensions.Json.Converters;

/// <summary>
/// Converts a <see cref="string"/> to and from JSON, trimming the value.
/// </summary>
public class StringTrimmingJsonConverter : JsonConverter<string>
{
    private readonly bool _trimToNull;

    /// <summary>
    /// Initializes a new instance of the <see cref="StringTrimmingJsonConverter"/> class.
    /// </summary>
    /// <param name="trimToNull">Decide, if emtpy strings should be replaced with null.</param>
    public StringTrimmingJsonConverter(bool trimToNull = true)
    {
        _trimToNull = trimToNull;
    }

    /// <inheritdoc/>
    public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        return Trim(value);
    }

    /// <inheritdoc/>
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
