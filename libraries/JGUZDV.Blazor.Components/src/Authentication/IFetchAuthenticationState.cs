using System.Security.Claims;

namespace JGUZDV.Blazor.Components.Authentication;

/// <summary>
/// Obsolete: Migrate to Components.Web and use PersistentAuthenticationStateProvider.
/// </summary>
[Obsolete("Migrate to Components.Web and use PersistentAuthenticationStateProvider")]
public interface IFetchAuthenticationState
{
    /// <summary>
    /// Obsolete: Migrate to Components.Web and use PersistentAuthenticationStateProvider.
    /// </summary>
    public Task<(ClaimsPrincipal? User, DateTimeOffset? ExpiresAt)> FetchPrincipalAsync(CancellationToken ct);
}
