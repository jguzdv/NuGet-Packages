using JGUZDV.Extensions.Json.Converters;
using System.Text.Json;

namespace JGUZDV.Extensions.Json.Tests;

public class StringTrimmingJsonConverterTest
{
    [Fact]
    public void TrimmedStrings_will_roundtrip_correctly_when_empty() 
    {
        var opt = new JsonSerializerOptions();
        opt.Converters.Add(new StringTrimmingJsonConverter(false));

        var original = "    ";
        var json = JsonSerializer.Serialize(original, opt);

        var actual = JsonSerializer.Deserialize<string>(json, opt);
        Assert.Equal("", actual);
    }
    
    [Fact]
    public void TrimmedStrings_will_roundtrip_correctly_when_null() 
    {
        var opt = new JsonSerializerOptions();
        opt.Converters.Add(new StringTrimmingJsonConverter(true));

        var original = "    ";
        var json = JsonSerializer.Serialize(original, opt);

        var actual = JsonSerializer.Deserialize<string>(json, opt);
        Assert.Null(actual);
    }
}