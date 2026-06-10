using System.ComponentModel;
using System.DirectoryServices.Protocols;
using System.Net;
using System.Text;

namespace JGUZDV.ActiveDirectory.Async;

/// <summary>
/// Provides asynchronous helper methods for creating LDAP connections, executing directory lookups,
/// and building LDAP-safe filter values.
/// </summary>
public static class LdapHelper
{
    /// <summary>
    /// Creates and binds an LDAP connection for the specified host and port using protocol version 3.
    /// </summary>
    /// <param name="host">The LDAP server host name or IP address.</param>
    /// <param name="port">The LDAP service port on the target server.</param>
    /// <param name="credential">Optional credentials used for binding; if <see langword="null"/>, the current security context is used.</param>
    /// <returns>A bound <see cref="LdapConnection"/> instance ready for LDAP operations.</returns>
    public static LdapConnection CreateConnection(string host, int port, NetworkCredential? credential = null)
    {
        var id = new LdapDirectoryIdentifier(host, port, true, false);
        var conn = new LdapConnection(id, credential, AuthType.Negotiate);
        conn.SessionOptions.ProtocolVersion = 3;
        conn.Bind();
        return conn;
    }

    /// <summary>
    /// Creates and binds an LDAP connection for the provided domain controller endpoint.
    /// </summary>
    /// <param name="endpoint">The domain controller endpoint containing host and port information.</param>
    /// <param name="credential">Optional credentials used for binding; if <see langword="null"/>, the current security context is used.</param>
    /// <returns>A bound <see cref="LdapConnection"/> instance ready for LDAP operations.</returns>
    public static LdapConnection CreateConnection(DomainControllerEndpoint endpoint, NetworkCredential? credential = null)
    {
        return CreateConnection(endpoint.HostName, endpoint.Port, credential);
    }

    /// <summary>
    /// Reads the default naming context from the root DSE of the connected directory.
    /// </summary>
    /// <param name="conn">The LDAP connection used to execute the root DSE query.</param>
    /// <param name="ct">A cancellation token for the asynchronous LDAP request.</param>
    /// <returns>A task that resolves to the default naming context distinguished name.</returns>
    public static async Task<string> GetDefaultNamingContextAsync(LdapConnection conn, CancellationToken ct)
    {
        var req = new SearchRequest("", "(objectClass=*)", SearchScope.Base, "defaultNamingContext");
        var res = await conn.SendRequestAsync<SearchResponse>(req, ct) ?? throw new InvalidOperationException();
        return (string)res.Entries[0].Attributes["defaultNamingContext"][0]!;
    }


    /// <summary>
    /// Searches for a directory entry by its object GUID and returns the first matching entry.
    /// </summary>
    /// <param name="conn">The LDAP connection used for the subtree search.</param>
    /// <param name="baseDn">The base distinguished name to search under; if empty, the default naming context is used.</param>
    /// <param name="guid">The object GUID of the directory entry to locate.</param>
    /// <param name="attributes">The list of attributes to load for the matching entry.</param>
    /// <param name="ct">A cancellation token for the asynchronous LDAP request.</param>
    /// <returns>A task that resolves to the first matching <see cref="SearchResultEntry"/>, or <see langword="null"/> if none is found.</returns>
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

    /// <summary>
    /// Resolves a distinguished name by GUID and performs a base-scope read on that entry.
    /// </summary>
    /// <param name="conn">The LDAP connection used for lookup and read operations.</param>
    /// <param name="baseDn">The base distinguished name for GUID lookup; if empty, the default naming context is used.</param>
    /// <param name="guid">The object GUID used to resolve the entry distinguished name.</param>
    /// <param name="attributes">The list of attributes to load from the resolved entry.</param>
    /// <param name="ct">A cancellation token for asynchronous LDAP requests.</param>
    /// <returns>A task that resolves to the entry read by distinguished name, or <see langword="null"/> if the entry cannot be resolved.</returns>
    public static async Task<SearchResultEntry?> DirectBindAsync(LdapConnection conn, string? baseDn, Guid guid, string[] attributes, CancellationToken ct)
    {
        var entry = await FindByGuidAsync(conn, baseDn, guid, ["distinguishedName"], ct);
        var bindDn = entry?.DistinguishedName;

        if (string.IsNullOrWhiteSpace(bindDn))
        {
            return null;
        }

        return await DirectBindAsync(conn, bindDn, attributes, ct);
    }

    /// <summary>
    /// Performs a base-scope LDAP read for the specified distinguished name.
    /// </summary>
    /// <param name="conn">The LDAP connection used to execute the base-scope search.</param>
    /// <param name="distinguishedName">The distinguished name of the entry to read.</param>
    /// <param name="attributes">The list of attributes to load from the entry.</param>
    /// <param name="ct">A cancellation token for the asynchronous LDAP request.</param>
    /// <returns>A task that resolves to the matching <see cref="SearchResultEntry"/>, or <see langword="null"/> when no valid entry is returned.</returns>
    public static async Task<SearchResultEntry?> DirectBindAsync(LdapConnection conn, string distinguishedName, string[] attributes, CancellationToken ct)
    {
        var req = new SearchRequest(distinguishedName, "(objectClass=*)", SearchScope.Base, attributes);
        var res = await conn.SendRequestAsync<SearchResponse>(req, ct);

        var result = res?.Entries.Count > 0 ? res.Entries[0] : null;
        return string.IsNullOrWhiteSpace(result?.DistinguishedName) ? null : result;
    }

    /// <summary>
    /// Converts a byte array into LDAP octet-string filter syntax.
    /// </summary>
    /// <param name="bytes">The byte sequence to encode.</param>
    /// <returns>A string formatted as escaped hexadecimal octets for LDAP filter usage.</returns>
    static string ToOctetFilter(byte[] bytes) => string.Concat(bytes.Select(b => $"\\{b:X2}"));

    /// <summary>
    /// Combines two filter expressions with the specified LDAP logical operator.
    /// </summary>
    /// <param name="op">The LDAP logical operator, typically '&amp;' for AND or '|' for OR.</param>
    /// <param name="filter1">The first LDAP filter expression.</param>
    /// <param name="filter2">The optional second LDAP filter expression.</param>
    /// <returns>The combined filter expression, or <paramref name="filter1"/> if <paramref name="filter2"/> is empty.</returns>
    public static string CombineFilter(char op, string filter1, string? filter2)
    {
        if (string.IsNullOrWhiteSpace(filter2))
        {
            return filter1;
        }

        return $"({op}{filter1}{filter2})";
    }

    /// <summary>
    /// Builds an LDAP OR filter expression for one attribute and multiple values.
    /// </summary>
    /// <param name="property">The LDAP attribute name used in each clause.</param>
    /// <param name="values">The values to include as OR clauses for the attribute.</param>
    /// <returns>An LDAP OR filter expression containing one clause per provided value.</returns>
    public static string BuildOrFilterExpression(string property, IEnumerable<string> values)
    {
        var orQuery = string.Join(null, values.Select(x => $"({property}={EncodeForFilter(x)})"));
        return $"(|{orQuery})";
    }

    /// <summary>
    /// Converts GUID values into LDAP-encoded hexadecimal strings suitable for filter usage.
    /// </summary>
    /// <param name="guids">The GUID values to encode.</param>
    /// <returns>An array of LDAP-encoded GUID strings.</returns>
    public static string[] ToSearchableGuidArray(Guid[] guids)
        => guids.Select(EncodeAsString).ToArray();

    /// <summary>
    /// Converts a GUID into an LDAP-encoded hexadecimal string.
    /// </summary>
    /// <param name="guid">The GUID value to encode.</param>
    /// <returns>An LDAP-encoded hexadecimal string representation of the GUID.</returns>
    public static string EncodeAsString(Guid guid)
        => EncodeAsString(guid.ToByteArray());

    /// <summary>
    /// Converts raw bytes into an LDAP-encoded hexadecimal string.
    /// </summary>
    /// <param name="bytes">The byte sequence to encode.</param>
    /// <returns>An LDAP-encoded hexadecimal string representation of the byte sequence.</returns>
    public static string EncodeAsString(byte[] bytes)
    {
        var hexBytes = bytes.Select(x => '\\' + x.ToString("x2"));
        return string.Concat(hexBytes);
    }

    /// <summary>
    /// Sanitizes and escapes input for safe inclusion in LDAP filter values.
    /// </summary>
    /// <param name="input">The input value to sanitize and encode.</param>
    /// <returns>A sanitized filter-safe string, or an empty string when the input has no usable content.</returns>
    public static string EncodeForFilter(string? input)
    {
        if (input == null)
            return "";

        var sb = new StringBuilder(input.Length * 2);
        var previousWasWhitespace = false;

        foreach (var rune in input.Trim().EnumerateRunes())
        {
            if (rune.Value == 0)
            {
                sb.Append("\\00");
                previousWasWhitespace = false;
                continue;
            }

            if (Rune.IsControl(rune))
            {
                continue;
            }

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

            if (rune.IsAscii)
            {
                sb.Append((char)rune.Value switch
                {
                    '*' => "\\2a",
                    '(' => "\\28",
                    ')' => "\\29",
                    '\\' => "\\5c",
                    _ => rune.ToString(),
                });
                continue;
            }

            sb.Append(rune);
        }

        var result = sb.ToString();
        return string.IsNullOrWhiteSpace(result) ? "" : result;
    }
}
