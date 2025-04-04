﻿using Microsoft.Extensions.Configuration;
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
    public static ConfigurationManager AddAsFirstJsonFile(this ConfigurationManager configuration, string path, bool optional = false, bool reloadOnChange = false)
    {
        var firstJson = configuration.Sources.OfType<JsonConfigurationSource>().FirstOrDefault();
        var index = firstJson != null ? configuration.Sources.IndexOf(firstJson) : 0;
        
        InsertJsonFile(configuration, path, optional, reloadOnChange, index);

        return configuration;
    }

    /// <summary>
    /// Adds a Json configuration source to the configuration builder after all other json files.
    /// </summary>
    public static ConfigurationManager AddAsLastJsonFile(this ConfigurationManager configuration, string path, bool optional = false, bool reloadOnChange = false)
    {
        var lastJson = configuration.Sources.OfType<JsonConfigurationSource>().LastOrDefault();
        var index = lastJson != null ? configuration.Sources.IndexOf(lastJson) + 1 : configuration.Sources.Count;
        
        InsertJsonFile(configuration, path, optional, reloadOnChange, index);

        return configuration;
    }

    /// <summary>
    /// Adds a Json configuration source to the configuration builder before a specific json file.
    /// </summary>
    public static bool TryAddJsonBeforeJsonFile(this ConfigurationManager configuration, string path, string otherPath, bool optional = false, bool reloadOnChange = false)
    {
        var otherJson = configuration.Sources.OfType<JsonConfigurationSource>().FirstOrDefault(x => x.Path == otherPath);
        if (otherJson == null)
        {
            return false;
        }

        var index = configuration.Sources.IndexOf(otherJson);
        InsertJsonFile(configuration, path, optional, reloadOnChange, index);
        
        return true;
    }

    /// <summary>
    /// Adds a Json configuration source to the configuration builder after a specific json file.
    /// </summary>
    public static bool TryAddJsonAfterJsonFile(this ConfigurationManager configuration, string path, string otherPath, bool optional = false, bool reloadOnChange = false)
    {
        var otherJson = configuration.Sources.OfType<JsonConfigurationSource>().FirstOrDefault(x => x.Path == otherPath);
        if (otherJson == null)
        {
            return false;
        }

        var index = configuration.Sources.IndexOf(otherJson) + 1;
        InsertJsonFile(configuration, path, optional, reloadOnChange, index);

        return true;
    }

    private static void InsertJsonFile(ConfigurationManager configuration, string path, bool optional, bool reloadOnChange, int index)
    {
        var source = new JsonConfigurationSource()
        {
            Path = path,
            Optional = optional,
            ReloadOnChange = reloadOnChange
        };
        source.ResolveFileProvider();

        configuration.Sources.Insert(index, source);
    }
}
