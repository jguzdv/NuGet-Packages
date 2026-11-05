using System.Text.Json;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace JGUZDV.L10n.EntityFramework;

/// <summary>
/// Converts <see cref="L10nString"/> to and from a JSON string for Entity Framework Core storage.
/// </summary>
public class L10nConverter : ValueConverter<L10nString, string>
{
    /// <inheritdoc />
    public L10nConverter()
        : base(
            modelValue => JsonSerializer.Serialize(modelValue, JsonSerializerOptions.Default),
            providerValue => JsonSerializer.Deserialize<L10nString>(providerValue, JsonSerializerOptions.Default) ?? new()
        )
    { }
}
