using System.Security.Claims;

using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;

namespace JGUZDV.Blazor.Components.Authentication;

[Obsolete("Migrate to Components.Web and use PersistentAuthenticationStateProvider")]
public class DelegatedAuthenticationStateProvider : AuthenticationStateProvider
{
    private static readonly AuthenticationState UnauthorizedUser = new(new (new ClaimsIdentity()));

    private readonly TimeProvider _timeProvider;
    private readonly IFetchAuthenticationState _fetchAuthenticationState;
    private readonly ILogger<DelegatedAuthenticationStateProvider> _logger;

    private DateTimeOffset _stateExpiry = DateTimeOffset.MinValue;
    private Task<AuthenticationState>? _authenticationState;

    public DelegatedAuthenticationStateProvider(
        TimeProvider timeProvider,
        IFetchAuthenticationState fetchAuthenticationState,
        ILogger<DelegatedAuthenticationStateProvider> logger)
    {
        _timeProvider = timeProvider;
        _fetchAuthenticationState = fetchAuthenticationState;
        _logger = logger;
    }

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        if(_authenticationState == null || _stateExpiry < _timeProvider.GetUtcNow())
        {

            _authenticationState = FetchAuthenticationStateAsync();
            NotifyAuthenticationStateChanged(_authenticationState);
        }

        return _authenticationState ?? Task.FromResult(UnauthorizedUser);
    }


    private async Task<AuthenticationState> FetchAuthenticationStateAsync()
    {
        var result = await _fetchAuthenticationState.FetchPrincipalAsync(default);

        _stateExpiry = result.ExpiresAt ?? DateTimeOffset.MinValue;

        return result.User != null 
            ? new AuthenticationState(result.User) 
            : UnauthorizedUser;
    }
}
