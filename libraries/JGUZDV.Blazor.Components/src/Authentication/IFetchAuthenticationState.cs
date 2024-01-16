using System.Security.Claims;

namespace JGUZDV.Blazor.Components.Authentication;

public interface IFetchAuthenticationState
{
    public Task<(ClaimsPrincipal? User, DateTimeOffset? ExpiresAt)> FetchPrincipalAsync(CancellationToken ct);
}
