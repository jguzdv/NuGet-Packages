using System.Text.Json;
using System.Text.Json.Serialization;

namespace JGUZDV.Extensions.Authorization
{
    public class ClaimRequirementJsonConverterFactory : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeof(ClaimRequirement).IsAssignableFrom(typeToConvert);
        }

        public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}