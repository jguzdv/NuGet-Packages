using System.DirectoryServices.Protocols;
using System.Net;

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

    static string ToOctetFilter(byte[] bytes) => string.Concat(bytes.Select(b => $"\\{b:X2}"));

    public static async Task<SearchResultEntry?> FindByGuidAsync(LdapConnection conn, string? baseDn, Guid guid, string[] attributes, CancellationToken ct)
    {
        if(string.IsNullOrWhiteSpace(baseDn))
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
        var entry = await FindByGuidAsync(conn, baseDn, guid, [], ct);

        var bindDn = entry?.DistinguishedName;
        var req = new SearchRequest(bindDn, "(objectClass=*)", SearchScope.Base, attributes);
        var res = await conn.SendRequestAsync<SearchResponse>(req, ct);
        return res?.Entries.Count > 0 ? res.Entries[0] : null;
    }
}
