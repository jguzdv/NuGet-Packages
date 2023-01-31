using System.Text.Json;

namespace JGUZDV.Extensions.Json.Tests
{
    public class JsonSerializerOptionsExtensionsTest
    {
        [Fact]
        public void Registeres_all_Converters()
        {
            var sut = new JsonSerializerOptions();
            sut.AddZDVDefaults();

            Assert.Null(sut.PropertyNamingPolicy);
            Assert.Null(sut.DictionaryKeyPolicy);
        }
    }
}
