using Microsoft.Extensions.Configuration;

namespace JGUZDV.AspNetCore.Hosting.Extensions
{
    internal static class ConfigurationExtensions
    {
        public static bool HasConfigSection(this IConfiguration configuration, string configSection)
            => configuration.GetSection(configSection).Exists();

        public static void ValidateConfigSectionExists(this IConfiguration configuration, string configSection)
        {
            if (!configuration.HasConfigSection(configSection))
            {
                throw new ConfigurationMissingException(configSection);
            }
        }
    }
}
