using System.Text.Json;
using System.Text.Json.Serialization;

using JGUZDV.DynamicForms.Extensions;

using Microsoft.Extensions.DependencyInjection;

namespace JGUZDV.DynamicForms.Tests;

internal static class TestJsonSerializerOptions
{
    public static JsonSerializerOptions Create()
    {
        var services = new ServiceCollection();
        services.AddDynamicForms();

        var serviceProvider = services.BuildServiceProvider();
        var options = serviceProvider.GetRequiredService<JsonSerializerOptions>();
        options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault;
        return options;
    }
}
