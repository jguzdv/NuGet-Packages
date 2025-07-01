using System.DirectoryServices;
using System.Runtime.Versioning;

/// <summary>
/// Provides extension methods for <see cref="PropertyValueCollection"/> to work with <see cref="ActiveDs.LargeInteger"/>.
/// </summary>
[SupportedOSPlatform("windows")]
public static class LargeIntegerExtensions
{
    /// <summary>
    /// Sets the value of the <see cref="PropertyValueCollection"/> to the specified <paramref name="value"/>.
    /// </summary>
    [SupportedOSPlatform("windows")]
    public static void SetLargeInteger(this PropertyValueCollection propertyValueCollection, long value)
    {
        var largeIntegerValue = new ActiveDs.LargeInteger
        {
            HighPart = (int)(value >> 32),
            LowPart = (int)(value & 0xFFFFFFFF)
        };

        propertyValueCollection.Value = largeIntegerValue;
    }

    /// <summary>
    /// Gets the value of the <see cref="PropertyValueCollection"/> as a <see cref="long"/>.
    /// </summary>
    [SupportedOSPlatform("windows")]
    public static long? GetLargeInteger(this PropertyValueCollection propertyValueCollection)
    {
        if (propertyValueCollection.Value is null)
        {
            return null;
        }

        var largeIntegerValue = (ActiveDs.LargeInteger)propertyValueCollection.Value;
        return (long)largeIntegerValue.HighPart << 32 | (uint)largeIntegerValue.LowPart;
    }

    /// <summary>
    /// Gets the value as a <see cref="long"/> from an object that is expected to be an <see cref="ActiveDs.LargeInteger"/>.
    /// </summary>
    public static long GetLargeInteger(object value)
    {
        var largeInteger = (ActiveDs.LargeInteger)value;
        return (long)largeInteger.HighPart << 32 | (uint)largeInteger.LowPart;
    }
}