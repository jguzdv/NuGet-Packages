using System.ComponentModel;
using System.DirectoryServices.Protocols;
using System.Net;
using System.Text;

namespace JGUZDV.ActiveDirectory.Async;

public static class LdapHelper
{
    public static LdapConnection CreateConnection(string host, int port, NetworkCredential? credential = null)
    {
        var id = new LdapDirectoryIdentifier(host, port, true, false);
        var conn = new LdapConnection(id, credential, AuthType.Negotiate);
        conn.SessionOptions.ProtocolVersion = 3;
        conn.Bind();
        return conn;
    }

    public static LdapConnection CreateConnection(DomainControllerEndpoint endpoint, NetworkCredential? credential = null)
    {
        return CreateConnection(endpoint.HostName, endpoint.Port, credential);
    }

    public static async Task<string> GetDefaultNamingContextAsync(LdapConnection conn, CancellationToken ct)
    {
        var req = new SearchRequest("", "(objectClass=*)", SearchScope.Base, "defaultNamingContext");
        var res = await conn.SendRequestAsync<SearchResponse>(req, ct) ?? throw new InvalidOperationException();
        return (string)res.Entries[0].Attributes["defaultNamingContext"][0]!;
    }


    public static async Task<SearchResultEntry?> FindByGuidAsync(LdapConnection conn, string? baseDn, Guid guid, string[] attributes, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(baseDn))
        {
            baseDn = await GetDefaultNamingContextAsync(conn, ct);
        }

        var filter = $"(objectGuid={ToOctetFilter(guid.ToByteArray())})";
        var req = new SearchRequest(baseDn, filter, SearchScope.Subtree, attributes);
        var res = await conn.SendRequestAsync<SearchResponse>(req, ct);
        return res?.Entries.Count > 0 ? res.Entries[0] : null;
    }

    public static async Task<SearchResultEntry?> DirectBind(LdapConnection conn, string? baseDn, Guid guid, string[] attributes, CancellationToken ct)
    {
        var entry = await FindByGuidAsync(conn, baseDn, guid, ["distinguishedName"], ct);
        var bindDn = entry?.DistinguishedName;

        if (string.IsNullOrWhiteSpace(bindDn))
        {
            return null;
        }

        return await DirectBind(conn, bindDn, attributes, ct);
    }

    public static async Task<SearchResultEntry?> DirectBind(LdapConnection conn, string distinguishedName, string[] attributes, CancellationToken ct)
    {
        var req = new SearchRequest(distinguishedName, "(objectClass=*)", SearchScope.Base, attributes);
        var res = await conn.SendRequestAsync<SearchResponse>(req, ct);

        var result = res?.Entries.Count > 0 ? res.Entries[0] : null;
        return string.IsNullOrWhiteSpace(result?.DistinguishedName) ? null : result;
    }

    static string ToOctetFilter(byte[] bytes) => string.Concat(bytes.Select(b => $"\\{b:X2}"));

    public static string CombineFilter(char op, string filter1, string? filter2)
    {
        if (string.IsNullOrWhiteSpace(filter2))
        {
            return filter1;
        }

        return $"({op}{filter1}{filter2})";
    }

    public static string BuildOrFilterExpression(string property, IEnumerable<string> values)
    {
        var orQuery = string.Join(null, values.Select(x => $"({property}={x})"));
        return $"(|{orQuery})";
    }

    public static string[] ToSearchableGuidArray(Guid[] guids)
        => guids.Select(EncodeAsString).ToArray();

    public static string EncodeAsString(Guid guid)
        => EncodeAsString(guid.ToByteArray());

    public static string EncodeAsString(byte[] bytes)
    {
        var hexBytes = bytes.Select(x => '\\' + x.ToString("x2"));
        return string.Concat(hexBytes);
    }

    public static string EncodeForFilter(string? input)
    {
        var result = SanitizeString(input);

        if (result == null)
            return "";

        return result;
    }

    private readonly static char[] _simpleEncodedChars = ['"', '#', '+', ',', ';', '<', '=', '>', '\\'];
    public static string? SanitizeString(string? input)
    {
        if (input == null)
            return null;

        var sb = new StringBuilder(input.Length * 2);

        var previousWasWhitespace = false;
        // UTF8 aware enumeration
        foreach (var rune in input.Trim().EnumerateRunes())
        {
            // Skip control characters
            if (Rune.IsControl(rune))
            {
                continue;
            }

            // All whitespace characters are replaced with a single space
            if (Rune.IsWhiteSpace(rune))
            {
                if (!previousWasWhitespace)
                {
                    sb.Append(' ');
                    previousWasWhitespace = true;
                }

                continue;
            }

            previousWasWhitespace = false;

            // Escape special characters
            if (rune.IsAscii && _simpleEncodedChars.Contains((char)rune.Value))
            {
                sb.Append('\\');
            }
            sb.Append(rune);
        }

        var result = sb.ToString();
        return string.IsNullOrWhiteSpace(result) ? null : result;
    }
}
