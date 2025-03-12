using System.Security.Claims;

namespace JGUZDV.Blazor.Components.Authentication;

[Obsolete("Migrate to Components.Web and use PersistentAuthenticationStateProvider")]
public interface IFetchAuthenticationState
{
    public Task<(ClaimsPrincipal? User, DateTimeOffset? ExpiresAt)> FetchPrincipalAsync(CancellationToken ct);
}
