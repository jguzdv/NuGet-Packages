using System.DirectoryServices;
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
    public string? ReadString(PropertyCollection properties, string propertyName, string? outputFormat = null)
    {
        var propertyValues = properties[propertyName];
        return propertyValues.Count != 0 
            ? ReadAsString(propertyValues[0]!, propertyName, outputFormat) 
            : default;
    }

    /// <summary>
    /// Reads an array of values from the property with the given name.
    /// </summary>
    public IEnumerable<string> ReadStrings(PropertyCollection properties, string propertyName, string? outputFormat = null)
    {
        var property = properties[propertyName];
        if (property.Value is null)
        {
            return Array.Empty<string>();
        }
        else if (property.Count == 1)
        {
            return [ReadAsString(properties[propertyName][0]!, propertyName, outputFormat)!];
        }
        else
        {
            return ((object[])property.Value!)
                .Select(x => ReadAsString(x, propertyName, outputFormat))
                .Where(x => x is not null)
                .ToArray()!;
        }
    }


    private string? ReadAsString(object propertyValue, string propertyName, string? outputFormat = null)
    {
        try
        {
            if (propertyValue is string stringValue)
            {
                return stringValue;
            }

            if (!_options.Value.PropertyInfos.TryGetValue(propertyName, out var propertyInfo))
            {
                _logger.LogDebug("No property info found for {PropertyName}, attempting string.Format() or ToString()", propertyName);
                return outputFormat != null
                    ? string.Format(outputFormat, propertyValue)
                    : propertyValue!.ToString();
            }

            if (propertyInfo.PropertyType == typeof(byte[]) && propertyValue is byte[] byteValue)
            {
                return outputFormat switch
                {
                    "Guid" => new Guid(byteValue).ToString(),
                    "SDDL" => new SecurityIdentifier(byteValue, 0).ToString(),
                    _ => Convert.ToBase64String(byteValue)
                };
            }

            if (propertyInfo.PropertyType == typeof(int) && propertyValue is int intValue)
            {
                return intValue.ToString(outputFormat ?? "0");
            }

            if (propertyInfo.PropertyType == typeof(long))
            {
                var longValue = ConvertIAdsLargeInteger(propertyValue);

                return outputFormat switch
                {
                    "FileTime" => DateTimeOffset.FromFileTime(longValue).ToString("O"),
                    _ => longValue.ToString(outputFormat ?? "0")
                };
            }

            if (propertyInfo.PropertyType == typeof(DateTime) && propertyValue is DateTime dateTimeValue)
            {
                return dateTimeValue.ToString(outputFormat ?? "O");
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to convert property {PropertyName} to string", propertyName);
        }

        return null;
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
        return propertyValues.Count != 0 ? ConvertIAdsLargeInteger(propertyValues[0]!) : default;
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
    public Guid ReadBytesAsGuid(PropertyCollection properties, string propertyName)
    {
        var propertyValues = properties[propertyName];
        return propertyValues.Count != 0 ? new Guid(ReadBytes(properties, propertyName)) : default;
    }

    /// <summary>
    /// Reads the given property as a byte array and converts it to a SecurityIdentifier.
    /// </summary>
    public SecurityIdentifier? ReadBytesSecurityIdentifier(PropertyCollection properties, string propertyName)
    {
        var propertyValues = properties[propertyName];
        return propertyValues.Count != 0 ? new SecurityIdentifier(ReadBytes(properties, propertyName), 0) : default;
    }

    /// <summary>
    /// Reads the given property as a long (file time) and converts it to a DateTimeOffset.
    /// </summary>
    public DateTimeOffset ReadLongAsDateTime(PropertyCollection properties, string propertyName)
    {
        var propertyValues = properties[propertyName];
        return propertyValues.Count != 0 ? DateTimeOffset.FromFileTime(ConvertIAdsLargeInteger(propertyValues[0]!)) : default;
    }


    private static long ConvertIAdsLargeInteger(object value)
    {
        var iadsLargeIntType = value.GetType();
        var highPart = (int)iadsLargeIntType.InvokeMember("HighPart", System.Reflection.BindingFlags.GetProperty, null, value, null)!;
        var lowPart = (uint)iadsLargeIntType.InvokeMember("LowPart", System.Reflection.BindingFlags.GetProperty, null, value, null)!;

        return ((long)highPart << 32) | lowPart;
    }
}