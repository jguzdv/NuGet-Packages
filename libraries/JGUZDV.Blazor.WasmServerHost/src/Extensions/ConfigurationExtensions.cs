﻿namespace JGUZDV.Blazor.WasmServerHost.Extensions
{
    internal static class ConfigurationExtensions
    {
        public static bool HasConfigSection(this IConfiguration configuration, string configSection)
            => configuration.GetSection(configSection).Exists();
    }
}
