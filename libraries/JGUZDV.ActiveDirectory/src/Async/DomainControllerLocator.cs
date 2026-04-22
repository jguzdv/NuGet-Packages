using DnsClient;

namespace JGUZDV.ActiveDirectory.Async;

public sealed record DomainControllerEndpoint(string HostName, int Port, int Priority, int Weight);

public static class DomainControllerLocator
{
    public static async Task<IReadOnlyList<DomainControllerEndpoint>> GetDomainControllersAsync(
        string domainName,
        string? siteName = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(domainName))
        {
            throw new ArgumentException("Domain name must not be null or whitespace.", nameof(domainName));
        }

        var normalizedDomain = domainName.Trim().TrimEnd('.');
        var normalizedSite = siteName?.Trim();
        var lookupClient = new LookupClient();

        foreach (var queryName in GetQueryNames(normalizedDomain, normalizedSite))
        {
            cancellationToken.ThrowIfCancellationRequested();

            var queryResult = await lookupClient.QueryAsync(
                queryName,
                QueryType.SRV,
                cancellationToken: cancellationToken);

            var records = queryResult.Answers
                .SrvRecords()
                .OrderBy(record => record.Priority)
                .ThenByDescending(record => record.Weight)
                .Select(record => new DomainControllerEndpoint(
                    record.Target.ToString().TrimEnd('.'),
                    record.Port,
                    record.Priority,
                    record.Weight))
                .DistinctBy(record => record.HostName, StringComparer.OrdinalIgnoreCase)
                .ToArray();

            if (records.Length > 0)
            {
                return records;
            }
        }

        return Array.Empty<DomainControllerEndpoint>();
    }

    private static IEnumerable<string> GetQueryNames(string domainName, string? siteName)
    {
        if (!string.IsNullOrWhiteSpace(siteName))
        {
            yield return $"_ldap._tcp.{siteName}._sites.dc._msdcs.{domainName}";
        }

        yield return $"_ldap._tcp.dc._msdcs.{domainName}";
    }
}
