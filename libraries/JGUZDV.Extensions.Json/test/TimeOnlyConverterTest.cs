using JGUZDV.Extensions.Json.Converters;
using System.Text.Json;

namespace JGUZDV.Extensions.Json.Tests;

public class TimeOnlyConverterTest
{
    [Fact]
    public void TimeOnly_will_roundtrip_correctly() 
    {
        var opt = new JsonSerializerOptions();
        opt.Converters.Add(new TimeOnlyConverter());

        var original = new TimeOnly(23, 25, 25);
        var json = JsonSerializer.Serialize(original, opt);

        var actual = JsonSerializer.Deserialize<TimeOnly>(json, opt);
        Assert.Equal(original, actual);
    }
}
