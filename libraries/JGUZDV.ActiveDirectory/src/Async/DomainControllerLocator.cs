using DnsClient;

namespace JGUZDV.ActiveDirectory.Async;

/// <summary>
/// Represents a domain controller endpoint discovered via DNS SRV records.
/// </summary>
/// <param name="HostName">The fully qualified host name of the domain controller.</param>
/// <param name="Port">The LDAP service port of the domain controller.</param>
/// <param name="Priority">The SRV priority value for the endpoint.</param>
/// <param name="Weight">The SRV weight value used for load balancing within the same priority.</param>
public sealed record DomainControllerEndpoint(string HostName, int Port, int Priority, int Weight);

/// <summary>
/// Defines an interface for asynchronously locating Active Directory domain controllers using DNS SRV records.
/// </summary>
public interface IDomainControllerLocator
{
    /// <summary>
    /// Asynchronously retrieves a list of domain controller endpoints for the specified Active Directory domain.
    /// </summary>
    /// <param name="domainName">The Active Directory domain name to query.</param>
    /// <param name="siteName">An optional Active Directory site name for site-aware discovery.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A read-only list of discovered domain controller endpoints, or an empty list if none are found.</returns>
    Task<IReadOnlyList<DomainControllerEndpoint>> GetDomainControllersAsync(
        string domainName,
        string? siteName = null,
        CancellationToken cancellationToken = default);
}


/// <summary>
/// Provides asynchronous DNS-based discovery for Active Directory domain controllers.
/// </summary>
public class DomainControllerLocator : IDomainControllerLocator
{
    private readonly ILookupClient _lookupClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="DomainControllerLocator"/> class with the specified DNS lookup client.
    /// </summary>
    /// <param name="lookupClient">The DNS lookup client to use for querying SRV records.</param>
    public DomainControllerLocator(ILookupClient lookupClient)
    {
        _lookupClient = lookupClient;
    }


    /// <summary>
    /// Resolves domain controller LDAP endpoints for the specified domain, optionally by a specific site.
    /// </summary>
    /// <param name="domainName">The Active Directory domain name to query.</param>
    /// <param name="siteName">An optional Active Directory site name used for site-aware lookup.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the lookup operation.</param>
    /// <returns>
    /// A read-only list of discovered domain controller endpoints ordered by SRV priority and weight,
    /// or an empty list if none are found.
    /// </returns>
    /// <exception cref="ArgumentException"><paramref name="domainName"/> is null, empty, or whitespace.</exception>
    public async Task<IReadOnlyList<DomainControllerEndpoint>> GetDomainControllersAsync(
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

        foreach (var queryName in GetQueryNames(normalizedDomain, normalizedSite))
        {
            cancellationToken.ThrowIfCancellationRequested();

            var queryResult = await _lookupClient.QueryAsync(
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

    /// <summary>
    /// Builds DNS SRV query names in lookup order, preferring a site-specific query when a site is provided.
    /// </summary>
    /// <param name="domainName">The normalized Active Directory domain name.</param>
    /// <param name="siteName">The normalized Active Directory site name, if available.</param>
    /// <returns>The ordered set of DNS SRV query names to execute.</returns>
    private static IEnumerable<string> GetQueryNames(string domainName, string? siteName)
    {
        if (!string.IsNullOrWhiteSpace(siteName))
        {
            yield return $"_ldap._tcp.{siteName}._sites.dc._msdcs.{domainName}";
        }

        yield return $"_ldap._tcp.dc._msdcs.{domainName}";
    }
}
