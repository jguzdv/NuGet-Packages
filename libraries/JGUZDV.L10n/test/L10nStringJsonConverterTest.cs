using System.Text.Json;

namespace JGUZDV.L10n.Tests;

public class L10nStringJsonConverterTest
{
    [Fact]
    public void Serialization_roundtrip()
    {
        var sut = new L10nString();
        sut["en"] = "English text";
        sut["de"] = "Deutscher Text";

        var jsonString = JsonSerializer.Serialize(sut);

        var asDic = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonString)!;
        var asL10n = JsonSerializer.Deserialize<L10nString>(jsonString)!;

        Assert.Equal("English text", asDic["en"]);
        Assert.Equal("English text", asL10n["en"]);
    }
}
