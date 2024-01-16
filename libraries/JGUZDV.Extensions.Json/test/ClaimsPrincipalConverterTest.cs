using System.Security.Claims;
using System.Text.Json;

using JGUZDV.Extensions.Json.Converters;

namespace JGUZDV.Extensions.Json.Tests
{
    public class ClaimsPrincipalConverterTest
    {
        [Fact]
        public void ClaimsPrincipal_will_roundtrip_correctly()
        {
            var opt = new JsonSerializerOptions();
            opt.Converters.Add(new ClaimsPrincipalConverter());

            var original = new ClaimsPrincipal(new[] {
                new ClaimsIdentity(new[] {
                    new Claim("t1", "v1"),
                    new Claim("t2", "v2")
                }, "test", "name", "role"),
                new ClaimsIdentity(new[]
                {
                    new Claim("x1", "x2")
                }, "test-amr", "displayName", "memberof")
            });
            var json = JsonSerializer.Serialize(original, opt);

            var actual = JsonSerializer.Deserialize<ClaimsPrincipal>(json, opt);
            Assert.Equal(2, actual.Identities.Count());
        }
    }
}
