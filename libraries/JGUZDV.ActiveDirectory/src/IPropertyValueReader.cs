using System.DirectoryServices;
using System.Security.Principal;

namespace JGUZDV.ActiveDirectory
{
    /// <summary>
    /// Reads property values from an <see cref="PropertyCollection"/>.
    /// </summary>
    public interface IPropertyValueReader
    {
        /// <summary>
        /// Read a byte array from the property with the given name.
        /// </summary>
        byte[] ReadBytes(PropertyCollection properties, string propertyName);

        /// <summary>
        /// Read a byte array from the property with the given name and convert it to a <see cref="Guid"/>.
        /// </summary>
        Guid ReadBytesAsGuid(PropertyCollection properties, string propertyName);

        /// <summary>
        /// Read a byte array from the property with the given name and convert it to a <see cref="SecurityIdentifier"/>.
        /// </summary>
        SecurityIdentifier? ReadBytesSecurityIdentifier(PropertyCollection properties, string propertyName);

        /// <summary>
        /// Read a <see cref="DateTime"/> from the property with the given name.
        /// </summary>
        DateTime ReadDateTime(PropertyCollection properties, string propertyName);

        /// <summary>
        /// Read an integer from the property with the given name.
        /// </summary>
        int ReadInt(PropertyCollection properties, string propertyName);

        /// <summary>
        /// Read a long from the property with the given name. (Converts IAdsLargeInteger to long)
        /// </summary>
        long ReadLong(PropertyCollection properties, string propertyName);

        /// <summary>
        /// Read a long from the property with the given name and convert it to a <see cref="DateTimeOffset"/>.
        /// </summary>
        DateTimeOffset ReadLongAsDateTime(PropertyCollection properties, string propertyName);

        /// <summary>
        /// Read a string from the property with the given name.
        /// If the property was not string it will be converted to string.
        /// </summary>
        string? ReadString(PropertyCollection properties, string propertyName, string? outputFormat = null);

        /// <summary>
        /// Read multiple strings from the property with the given name.
        /// </summary>
        IEnumerable<string> ReadStrings(PropertyCollection properties, string propertyName, string? outputFormat = null);
    }
}