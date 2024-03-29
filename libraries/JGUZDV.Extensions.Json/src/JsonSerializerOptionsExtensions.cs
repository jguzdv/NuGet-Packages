﻿using JGUZDV.Extensions.Json.Converters;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace JGUZDV.Extensions.Json;

/// <summary>
/// Extension methods for <see cref="JsonSerializerOptions"/>.
/// </summary>
public static class JsonSerializerOptionsExtensions
{
    /// <summary>
    /// This will set some default behaviour to JsonSerializerOptions:
    /// - if NET6_0:
    ///   - Add DateOnlyConverter
    ///   - Add TimeOnlyConverter
    /// - always:
    /// - Add StringTrimmingJsonConverter
    /// - Set DefaultIgnoreCondition to "WhenWritingNull"
    /// - PropertyNamingPolicy to 'null'
    /// - DictonaryKeyPolicy to 'null'
    /// </summary>
    public static void SetJGUZDVDefaults(this JsonSerializerOptions opt)
    {
#if NET6_0
        opt.Converters.Add(new DateOnlyConverter());
        opt.Converters.Add(new TimeOnlyConverter());
#endif
        opt.Converters.Add(new StringTrimmingJsonConverter());

        opt.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        opt.PropertyNamingPolicy = null;
        opt.DictionaryKeyPolicy = null;
    }
}
