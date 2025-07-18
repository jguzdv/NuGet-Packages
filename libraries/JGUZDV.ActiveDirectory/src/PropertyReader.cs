﻿using System.DirectoryServices;
using System.Runtime.Versioning;
using System.Security.Principal;

using JGUZDV.ActiveDirectory.Configuration;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace JGUZDV.ActiveDirectory;

/// <summary>
/// Reads property values from an <see cref="PropertyCollection"/>.
/// </summary>
[SupportedOSPlatform("windows")]
internal class PropertyReader(
    IOptions<PropertyReaderOptions> options,
    ILogger<PropertyReader> logger) : IPropertyReader
{
    private readonly IOptions<PropertyReaderOptions> _options = options;
    private readonly ILogger<PropertyReader> _logger = logger;

    /// <summary>
    /// Reads a value from the property with the given name.
    /// </summary>
    /// <returns>The first element from the property value collection, if it's not string, conversion will occur and the outputFormat will be applied, if possible.</returns>
    public string? ReadString(PropertyCollection properties, string propertyName, string? outputFormat, Casing casing)
    {
        var propertyValues = properties[propertyName];
        return propertyValues.Count != 0 
            ? ReadAsString(propertyValues[0]!, propertyName, outputFormat, casing) 
            : default;
    }

    /// <summary>
    /// Reads an array of values from the property with the given name.
    /// </summary>
    public IEnumerable<string> ReadStrings(PropertyCollection properties, string propertyName, string? outputFormat, Casing casing)
    {
        var property = properties[propertyName];
        if (property.Value is null)
        {
            return Array.Empty<string>();
        }
        
        if (property.Count == 1)
        {
            return [ReadAsString(properties[propertyName][0]!, propertyName, outputFormat, casing)!];
        }
        else
        {
            return ((object[])property.Value!)
                .Select(x => ReadAsString(x, propertyName, outputFormat, casing))
                .Where(x => x is not null)
                .ToArray()!;
        }
    }


    private string? ReadAsString(object propertyValue, string propertyName, string? outputFormat, Casing casing)
    {
        try
        {
            string? result = null;

            if (propertyValue is string stringValue)
            {
                if (OutputFormats.ADStrings.CN.Equals(outputFormat, StringComparison.OrdinalIgnoreCase))
                {
                    if(!stringValue.StartsWith("CN=", StringComparison.OrdinalIgnoreCase))
                    {
                        _logger.LogWarning("Output format {OutputFormat} is not applicable to {PropertyName}", outputFormat, propertyName);
                        return stringValue;
                    }

                    result = stringValue.Split(',', 2, StringSplitOptions.TrimEntries).First()
                        .Split('=', 2, StringSplitOptions.TrimEntries).Last();
                }
                else
                {
                    result = stringValue;
                }
            }

            else if (!_options.Value.PropertyInfos.TryGetValue(propertyName, out var propertyInfo))
            {
                _logger.LogDebug("No property info found for {PropertyName}, attempting string.Format() or ToString()", propertyName);
                result = outputFormat != null
                    ? string.Format(outputFormat, propertyValue)
                    : propertyValue!.ToString();
            }


            else if (propertyInfo.PropertyType == typeof(byte[]) && propertyValue is byte[] byteValue)
            {
                if (OutputFormats.ByteArrays.Guid.Equals(outputFormat, StringComparison.OrdinalIgnoreCase))
                {
                    result = new Guid(byteValue).ToString();
                }
                else if (OutputFormats.ByteArrays.SDDL.Equals(outputFormat, StringComparison.OrdinalIgnoreCase))
                {
                    result = new SecurityIdentifier(byteValue, 0).ToString();
                }
                else
                {
                    result = Convert.ToBase64String(byteValue);
                }
            }


            else if (propertyInfo.PropertyType == typeof(int) && propertyValue is int intValue)
            {
                result = intValue.ToString(outputFormat ?? "0");
            }


            else if (propertyInfo.PropertyType == typeof(long))
            {
                var longValue = LargeIntegerExtensions.GetLargeInteger(propertyValue);

                if (OutputFormats.Long.FileTime.Equals(outputFormat, StringComparison.OrdinalIgnoreCase))
                {
                    result = DateTimeOffset.FromFileTime(longValue).ToString("O");
                }
                else 
                {
                    result = longValue.ToString(outputFormat ?? "0");
                }
            }

            else if (propertyInfo.PropertyType == typeof(DateTime) && propertyValue is DateTime dateTimeValue)
            {
                result = dateTimeValue.ToString(outputFormat ?? "O");
            }

            return casing switch
            {
                Casing.Lower => result?.ToLowerInvariant(),
                Casing.Upper => result?.ToUpperInvariant(),
                _ => result
            };
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to convert property {PropertyName} to string", propertyName);
            return null;
        }
    }

    /// <summary>
    /// Reads the given property as an integer.
    /// </summary>
    public int ReadInt(PropertyCollection properties, string propertyName)
    {
        var propertyValues = properties[propertyName];
        return propertyValues.Count != 0 ? (int)propertyValues[0]! : default;
    }


    /// <summary>
    /// Reads the given property as a long.
    /// </summary>
    public long ReadLong(PropertyCollection properties, string propertyName)
    {
        var propertyValues = properties[propertyName];
        return propertyValues.Count != 0 ? propertyValues.GetLargeInteger()!.Value : default;
    }


    /// <summary>
    /// Reads the given property as a DateTime.
    /// </summary>
    public DateTime ReadDateTime(PropertyCollection properties, string propertyName)
    {
        var propertyValues = properties[propertyName];
        return propertyValues.Count != 0 ? (DateTime)propertyValues[0]! : default;
    }

    /// <summary>
    /// Reads the given property as a byte array.
    /// </summary>
    public byte[] ReadBytes(PropertyCollection properties, string propertyName)
    {
        var propertyValues = properties[propertyName];
        return propertyValues.Count != 0 
            ? (byte[])propertyValues[0]! 
            : [];
    }


    /// <summary>
    /// Reads the given property as a byte array and converts it to guid.
    /// </summary>
    public Guid? ReadBytesAsGuid(PropertyCollection properties, string propertyName)
    {
        try
        {
            var propertyValues = properties[propertyName];
            return propertyValues.Count == 1 ? new Guid(ReadBytes(properties, propertyName)) : null;
        }
        catch (Exception e)
        {
            _logger.LogWarning(e, "Failed to convert property {PropertyName} to Guid", propertyName);
            return default;
        }
    }

    /// <summary>
    /// Reads the given property as a byte array and converts it to a SecurityIdentifier.
    /// </summary>
    public SecurityIdentifier? ReadBytesSecurityIdentifier(PropertyCollection properties, string propertyName)
    {
        try { 
            var propertyValues = properties[propertyName];
            return propertyValues.Count != 0 ? new SecurityIdentifier(ReadBytes(properties, propertyName), 0) : default;
        }
        catch (Exception e)
        {
            _logger.LogWarning(e, "Failed to convert property {PropertyName} to SecurityIdentifier", propertyName);
            return default;
        }
    }

    /// <summary>
    /// Reads the given property as a long (file time) and converts it to a DateTimeOffset.
    /// </summary>
    public DateTimeOffset ReadLongAsDateTime(PropertyCollection properties, string propertyName)
    {
        var propertyValues = properties[propertyName];
        return propertyValues.Count != 0 ? DateTimeOffset.FromFileTime(propertyValues.GetLargeInteger()!.Value) : default;
    }
}