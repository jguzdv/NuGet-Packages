using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

namespace JGUZDV.AspNetCore.Hosting.Configuration;

/// <summary>
/// Provides extension methods for configuration classes.
/// </summary>
public static class ConfigurationExtensions
{
    /// <summary>
    /// Adds a Json configuration source to the configuration builder before all other json files.
    /// </summary>
    public static ConfigurationManager AddJsonFileBeforeOthers(this ConfigurationManager configuration, string path, bool optional = false, bool reloadOnChange = false)
    {
        var firstJson = configuration.Sources.FirstOrDefault(x => x.GetType() == typeof(JsonConfigurationSource));
        var index = firstJson != null ? configuration.Sources.IndexOf(firstJson) : 0;

        var source = new JsonConfigurationSource()
        {
            Path = path,
            Optional = optional,
            ReloadOnChange = reloadOnChange
        };
        source.ResolveFileProvider();

        configuration.Sources.Insert(index, source);
        return configuration;
    }

    /// <summary>
    /// Adds a Json configuration source to the configuration builder after all other json files.
    /// </summary>
    public static ConfigurationManager AddJsonFileAfterOthers(this ConfigurationManager configuration, string path, bool optional = false, bool reloadOnChange = false)
    {
        var lastJson = configuration.Sources.LastOrDefault(x => x.GetType() == typeof(JsonConfigurationSource));
        var index = lastJson != null ? configuration.Sources.IndexOf(lastJson) + 1 : configuration.Sources.Count;
        var source = new JsonConfigurationSource()
        {
            Path = path,
            Optional = optional,
            ReloadOnChange = reloadOnChange
        };
        source.ResolveFileProvider();


        configuration.Sources.Insert(index, source);
        return configuration;
    }
}