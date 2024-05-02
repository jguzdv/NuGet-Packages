using System.DirectoryServices;
using System.Security.Principal;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace JGUZDV.ActiveDirectory;

public class PropertyValueReader
{
    private readonly IOptions<PropertyReaderOptions> _options;
    private readonly ILogger<PropertyValueReader> _logger;

    public PropertyValueReader(
        IOptions<PropertyReaderOptions> options,
        ILogger<PropertyValueReader> logger)
    {
        _options = options;
        _logger = logger;
    }

    /// <summary>
    /// Reads a value from the property with the given name.
    /// </summary>
    /// <typeparam name="T">If T does not match the native type, a type conversion will occur.</typeparam>
    /// <returns>The first element from the property value collection, converted to the requested target type.</returns>
    public string? ReadString(PropertyCollection properties, string propertyName, string? outputFormat = null)
    {
        var propertyValues = properties[propertyName];
        if (propertyValues.Count == 0)
        {
            return default;
        }

        if (propertyValues[0] is string stringValue)
        {
            return stringValue;
        }

        return ReadAsString(propertyValues[0]!, propertyName, outputFormat);
    }


    public IEnumerable<string> ReadStrings(PropertyCollection properties, string propertyName, string? outputFormat = null)
    {
        if (!(properties[propertyName] is { Count: > 0 } and { Value: object[] propertyValues }))
        {
            return Array.Empty<string>();
        }

        return propertyValues.Select(x => ReadAsString(x, propertyName, outputFormat)).ToArray();
    }

    private string? ReadAsString(object propertyValue, string propertyName, string? outputFormat = null)
    {
        if (propertyValue is string stringValue)
        {
            return stringValue;
        }

        return "";

        if (!_options.Value.PropertyInfos.TryGetValue(propertyName, out var propertyInfo))
        {
            _logger.LogDebug("No property info found for {PropertyName}, attempting string.Format() or ToString()", propertyName);
            return outputFormat != null
                ? string.Format(outputFormat, propertyValue)
                : propertyValue!.ToString();
        }
    }

    /// <summary>
    /// Reads the given property as an integer.
    /// </summary>
    public static int ReadInt(PropertyCollection properties, string propertyName)
    {
        var propertyValues = properties[propertyName];
        return propertyValues.Count != 0 ? (int)propertyValues[0]! : default;
    }


    /// <summary>
    /// Reads the given property as a long.
    /// </summary>
    public static long ReadLong(PropertyCollection properties, string propertyName)
    {
        var propertyValues = properties[propertyName];
        return propertyValues.Count != 0 ? ConvertIAdsLargeInteger(propertyValues[0]!) : default;
    }


    /// <summary>
    /// Reads the given property as a DateTime.
    /// </summary>
    public static DateTime ReadDateTime(PropertyCollection properties, string propertyName)
    {
        var propertyValues = properties[propertyName];
        return propertyValues.Count != 0 ? (DateTime)propertyValues[0]! : default;
    }

    /// <summary>
    /// Reads the given property as a byte array.
    /// </summary>
    public static byte[] ReadBytes(PropertyCollection properties, string propertyName)
    {
        var propertyValues = properties[propertyName];
        return propertyValues.Count != 0 ? (byte[])propertyValues[0]! : Array.Empty<byte>();
    }


    /// <summary>
    /// Reads the given property as a byte array and converts it to guid.
    /// </summary>
    public static Guid ReadBytesAsGuid(PropertyCollection properties, string propertyName)
    {
        var propertyValues = properties[propertyName];
        return propertyValues.Count != 0 ? new Guid(ReadBytes(properties, propertyName)) : default;
    }

    /// <summary>
    /// Reads the given property as a byte array and converts it to a SecurityIdentifier.
    /// </summary>
    public static SecurityIdentifier? ReadBytesSecurityIdentifier(PropertyCollection properties, string propertyName)
    {
        var propertyValues = properties[propertyName];
        return propertyValues.Count != 0 ? new SecurityIdentifier(ReadBytes(properties, propertyName), 0) : default;
    }

    /// <summary>
    /// Reads the given property as a long (file time) and converts it to a DateTimeOffset.
    /// </summary>
    public static DateTimeOffset ReadLongAsDateTime(PropertyCollection properties, string propertyName)
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