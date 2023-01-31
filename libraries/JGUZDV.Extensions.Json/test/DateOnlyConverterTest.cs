using JGUZDV.Extensions.Json.Converters;
using System.Text.Json;

namespace JGUZDV.Extensions.Json.Tests;

public class DateOnlyConverterTest
{
    [Fact]
    public void DateOnly_will_roundtrip_correctly()
    {
        var opt = new JsonSerializerOptions();
        opt.Converters.Add(new DateOnlyConverter());

        var original = new DateOnly(2000, 10, 15);
        var json = JsonSerializer.Serialize(original, opt);

        var actual = JsonSerializer.Deserialize<DateOnly>(json, opt);
        Assert.Equal(original, actual);
    }
}
