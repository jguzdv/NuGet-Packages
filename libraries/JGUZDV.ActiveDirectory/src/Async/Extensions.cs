using System.DirectoryServices.Protocols;
using System.Globalization;

namespace JGUZDV.ActiveDirectory.Async
{
    public static class LdapConnectionExtensions
    {
        extension(LdapConnection connection)
        {
            /// <summary>
            /// 
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="request"></param>
            /// <param name="ct"></param>
            /// <returns></returns>
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

    public static class SearchResultEntryExtensions
    {
        extension(SearchResultEntry entry)
        {
            public string? GetString(string attributeName)
            {
                return entry.Attributes[attributeName].Cast<string>().FirstOrDefault();
            }

            public string[] GetStrings(string attributeName)
            {
                var attribute = entry.Attributes[attributeName];
                if (attribute is null)
                {
                    return Array.Empty<string>();
                }
                return attribute.Cast<string>().ToArray();
            }

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

            public byte[] GetBytes(string attributeName)
            {
                var attribute = entry.Attributes[attributeName];
                if (attribute is null)
                {
                    return Array.Empty<byte>();
                }

                return attribute.Cast<object>().FirstOrDefault() as byte[] ?? Array.Empty<byte>();
            }

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
