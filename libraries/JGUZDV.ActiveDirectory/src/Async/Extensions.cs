using System.DirectoryServices.Protocols;
using System.Globalization;

namespace JGUZDV.ActiveDirectory.Async
{
    /// <summary>
    /// Provides asynchronous helper methods for <see cref="LdapConnection"/> operations.
    /// </summary>
    public static class LdapConnectionExtensions
    {
        extension(LdapConnection connection)
        {
            /// <summary>
            /// Sends an LDAP request asynchronously and returns a strongly typed directory response.
            /// </summary>
            /// <typeparam name="T">The expected response type.</typeparam>
            /// <param name="request">The LDAP request to send.</param>
            /// <param name="ct">A token used to cancel the underlying LDAP operation.</param>
            /// <returns>The LDAP response cast to <typeparamref name="T"/>.</returns>
            public async Task<T> SendRequestAsync<T>(DirectoryRequest request, CancellationToken ct)
                where T : DirectoryResponse
            {
                var asyncResult = connection.BeginSendRequest(request, PartialResultProcessing.NoPartialResultSupport, null, null);
                using var ctr = ct.Register(() => connection.Abort(asyncResult));

                return (T) await Task.Factory.FromAsync(
                    asyncResult,
                    connection.EndSendRequest
                );
            }
        }
    }

    /// <summary>
    /// Provides typed attribute access helpers for <see cref="SearchResultEntry"/> values.
    /// </summary>
    public static class SearchResultEntryExtensions
    {
        extension(SearchResultEntry entry)
        {
            /// <summary>
            /// Gets the first string value of the specified LDAP attribute.
            /// </summary>
            /// <param name="attributeName">The name of the attribute to read.</param>
            /// <returns>The first string value, or <see langword="null"/> if the attribute is missing or has no string values.</returns>
            public string? GetString(string attributeName)
            {
                var attribute = entry.Attributes[attributeName];
                if (attribute is null)
                {
                    return null;
                }
                return attribute.GetValues(typeof(string)).Cast<string>().FirstOrDefault();
            }

            /// <summary>
            /// Gets all string values of the specified LDAP attribute.
            /// </summary>
            /// <param name="attributeName">The name of the attribute to read.</param>
            /// <returns>An array containing all string values, or an empty array if the attribute is missing.</returns>
            public string[] GetStrings(string attributeName)
            {
                var attribute = entry.Attributes[attributeName];
                if (attribute is null)
                {
                    return Array.Empty<string>();
                }
                return attribute.GetValues(typeof(string)).Cast<string>().ToArray();
            }

            /// <summary>
            /// Gets the first value of the specified LDAP attribute as an integer.
            /// </summary>
            /// <param name="attributeName">The name of the attribute to read.</param>
            /// <returns>The parsed integer value, or <see langword="null"/> if the attribute is missing or cannot be parsed as an integer.</returns>
            public int? GetInt(string attributeName)
            {
                var attribute = entry.Attributes[attributeName];
                if (attribute is null)
                {
                    return null;
                }

                var value = attribute.Cast<object>().FirstOrDefault();
                if (value is int intValue)
                {
                    return intValue;
                }

                if (value is string stringValue && int.TryParse(stringValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsedInt))
                {
                    return parsedInt;
                }

                return null;
            }

            /// <summary>
            /// Gets all values of the specified LDAP attribute that can be represented as integers.
            /// </summary>
            /// <param name="attributeName">The name of the attribute to read.</param>
            /// <returns>An array of successfully converted integer values, or an empty array if the attribute is missing.</returns>
            public int[] GetInts(string attributeName)
            {
                var attribute = entry.Attributes[attributeName];
                if (attribute is null)
                {
                    return Array.Empty<int>();
                }

                return attribute
                    .Cast<object>()
                    .Select(v => v switch
                    {
                        int i => i,
                        string s when int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed) => parsed,
                        _ => (int?)null,
                    })
                    .Where(v => v.HasValue)
                    .Select(v => v!.Value)
                    .ToArray();
            }

            /// <summary>
            /// Gets the first value of the specified LDAP attribute as a byte array.
            /// </summary>
            /// <param name="attributeName">The name of the attribute to read.</param>
            /// <returns>The first byte array value, or an empty array if the attribute is missing or not binary.</returns>
            public byte[] GetBytes(string attributeName)
            {
                var attribute = entry.Attributes[attributeName];
                if (attribute is null)
                {
                    return Array.Empty<byte>();
                }

                return attribute.Cast<object>().FirstOrDefault() as byte[] ?? Array.Empty<byte>();
            }

            /// <summary>
            /// Gets the first value of the specified LDAP attribute as a <see cref="Guid"/>.
            /// </summary>
            /// <param name="attributeName">The name of the attribute to read.</param>
            /// <returns>A GUID parsed from a 16-byte binary value or string value, or <see langword="null"/> if conversion is not possible.</returns>
            public Guid? GetGuid(string attributeName)
            {
                var attribute = entry.Attributes[attributeName];
                if (attribute is null)
                {
                    return null;
                }

                var value = attribute.Cast<object>().FirstOrDefault();
                return value switch
                {
                    byte[] bytes when bytes.Length == 16 => new Guid(bytes),
                    string s when Guid.TryParse(s, out var guid) => guid,
                    _ => null,
                };
            }
        }
    }
}
