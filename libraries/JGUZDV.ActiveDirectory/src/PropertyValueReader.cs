using System.DirectoryServices;

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

        return ReadAsString(propertyValues[0], propertyName, outputFormat);
    }


    public IEnumerable<string> ReadStrings(PropertyCollection properties, string propertyName, string? outputFormat = null)
    {
        if (!(properties[propertyName] is { Count: > 0 } and { Value: object[] propertyValues }))
        {
            return Array.Empty<string>();
        }

        var convertProperty = GetToStringConversion(typeof(string), propertyName);
        return propertyValues.Select((x,i) => );
    }

    private string? ReadAsString(object propertyValue, string propertyName, string? outputFormat = null)
    {
        

        if (!_options.Value.PropertyInfos.TryGetValue(propertyName, out var propertyInfo))
        {
            _logger.LogDebug("No property info found for {PropertyName}, attempting string.Format() or ToString()", propertyName);
            return outputFormat != null
                ? string.Format(outputFormat, propertyValue)
                : propertyValue!.ToString();
        }
    }

    /// <summary>
    /// Reads all values from the property with the given name.
    /// </summary>
    /// <typeparam name="T">If T does not match the native type, a type conversion will occur.</typeparam>
    public IEnumerable<T> ReadValues<T>(PropertyCollection properties, string propertyName)
    {
        if (!(properties[propertyName] is { Count: >0 } and { Value: object[] propertyValues }))
        {
            return Array.Empty<T>();
        }

        var convertProperty = GetPropertyConversion(typeof(T), propertyName);
        return propertyValues.Select(x => (T)convertProperty(x!));
    }


    private Func<object, string> GetToStringConversion(Type sourceType, string propertyName)
    {
        if (!_options.Value.PropertyInfos.TryGetValue(propertyName, out var propertyInfo))
        {
            _logger.LogDebug("No property info found for {PropertyName}, a cast will be used for conversion.", propertyName);
        }

        if (propertyInfo == null || targetType == propertyInfo.PropertyType)
        {
            if (targetType == typeof(long))
            {
                return x => ConvertIAdsLargeInteger(x);
            }
        }

        return x => x;
    }

    private static long ConvertIAdsLargeInteger(object value)
    {
        var iadsLargeIntType = value.GetType();
        var highPart = (int)iadsLargeIntType.InvokeMember("HighPart", System.Reflection.BindingFlags.GetProperty, null, value, null);
        var lowPart = (uint)iadsLargeIntType.InvokeMember("LowPart", System.Reflection.BindingFlags.GetProperty, null, value, null);
        return ((long)highPart << 32) | lowPart;
    }
}