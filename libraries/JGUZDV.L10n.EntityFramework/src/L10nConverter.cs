using System.Text.Json;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace JGUZDV.L10n.EntityFramework;

public class L10nConverter : ValueConverter<L10nString, string>
{
    public L10nConverter()
        : base(
            modelValue => JsonSerializer.Serialize(modelValue, JsonSerializerOptions.Default),
            providerValue => JsonSerializer.Deserialize<L10nString>(providerValue, JsonSerializerOptions.Default) ?? new()
        )
    { }
}
