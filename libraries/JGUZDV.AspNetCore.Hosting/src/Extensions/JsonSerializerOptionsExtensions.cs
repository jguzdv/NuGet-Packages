using System.Text.Json;
using System.Text.Json.Serialization;

namespace JGUZDV.AspNetCore.Hosting.Extensions
{
    internal static class JsonSerializerOptionsExtensions
    {
        public static void SetJGUZDVDefaults(this JsonSerializerOptions opt)
        {
            opt.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            opt.PropertyNamingPolicy = null;
            opt.DictionaryKeyPolicy = null;
        }
    }
}
