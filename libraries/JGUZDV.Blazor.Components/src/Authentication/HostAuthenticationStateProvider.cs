using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace JGUZDV.Blazor.Components.Authentication;

public abstract class HostAuthenticationStateProvider<THostUser> : AuthenticationStateProvider, IDisposable
{
    private static readonly ClaimsPrincipal UnauthorizedUser =
        new ClaimsPrincipal(new ClaimsIdentity());

    private readonly IOptions<HostAuthenticationOptions> _options;
    private readonly ILogger<HostAuthenticationStateProvider<THostUser>> _logger;
    
    private readonly Timer _timer;
    
    private DateTimeOffset _nextFetchAfter;
    private ClaimsPrincipal? _cachedPrincipal;

    private Task<AuthenticationState>? _cachedAuthenticationState;

    public HostAuthenticationStateProvider(
        IOptions<HostAuthenticationOptions> options,
        ILogger<HostAuthenticationStateProvider<THostUser>> logger)
    {
        _options = options;
        _logger = logger;
        var pollInterval = TimeSpan.FromSeconds(_options.Value.PollIntervalSeconds);

        _timer = new Timer(
            _ => ExecuteTimer(),
            null, pollInterval, pollInterval);
    }

    public void Dispose()
    {
        _timer.Dispose();
    }

    public void ExecuteTimer()
    {
        _ = FetchPrincipalAsync(true);
    }

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        if(_cachedAuthenticationState != null)
            return _cachedAuthenticationState;

        _cachedAuthenticationState = GetAuthenticationStateInternalAsync();
        return _cachedAuthenticationState;
    }

    public async Task<AuthenticationState> GetAuthenticationStateInternalAsync()
    {
        if (_cachedPrincipal != null)
            return new AuthenticationState(_cachedPrincipal);

        if (_cachedPrincipal?.Identity?.IsAuthenticated == true)
        {
            if (DateTimeOffset.Now > _nextFetchAfter)
            {
                _logger.LogInformation("Cached principal is stale. Refreshing in background ...");
                _ = FetchPrincipalAsync(notifyOnChange: true);
            }
        }
        else
        {
            _logger.LogInformation("Cached principal was unauthenticated. Wait for cache refresh ...");
            await FetchPrincipalAsync(notifyOnChange: false);
        }

        return new AuthenticationState(_cachedPrincipal!);
    }

    private async Task FetchPrincipalAsync(bool notifyOnChange)
    {
        var fetchResult = await FetchUserAsync();
        bool hasChanges;

        if (!fetchResult.IsAuthenticated || fetchResult.User == null)
        {
            _logger.LogInformation("FetchUserAsync() returned null. User is not authenticated.");
            hasChanges = UpdateCaches(UnauthorizedUser, DateTimeOffset.Now.AddDays(1));
        }
        else
        {
            _logger.LogInformation("Successfully fetched user.");
            
            var claims = GetClaimsFromUser(fetchResult.User) ?? Enumerable.Empty<Claim>();
            
            var identity = new ClaimsIdentity(claims, "HostProvidedIdentity", 
                _options.Value.NameClaimType, _options.Value.RoleClaimType);
            var newPrincipal = new ClaimsPrincipal(identity);

            var expiresAt = GetAbsoluteExpiry(fetchResult.User);

            hasChanges = UpdateCaches(newPrincipal, expiresAt);
        }

        if (notifyOnChange && hasChanges)
        {
            _logger.LogInformation("Principal has changed, send notification on AuthenticationState change");
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_cachedPrincipal!)));
        }
    }

    private bool UpdateCaches(ClaimsPrincipal newPrincipal, DateTimeOffset? expiresAt)
    {
        var oldPrincipal = _cachedPrincipal;
        var hasPrincipalChanged = HasPrincipalChanged(oldPrincipal, newPrincipal);

        _cachedPrincipal = newPrincipal;

        var cacheRefreshTime = DateTimeOffset.Now.AddSeconds(_options.Value.CacheRefreshIntervalSeconds);
        if (!expiresAt.HasValue) {
            _nextFetchAfter = cacheRefreshTime;
        }
        else
        {
            _nextFetchAfter = new DateTimeOffset(
                Math.Min(expiresAt.Value.UtcTicks, cacheRefreshTime.UtcTicks),
                TimeSpan.Zero
            );
        }

        _logger.LogInformation($"Updated cached principal. NextFetch: {_nextFetchAfter}.");
        return hasPrincipalChanged;
    }

    private bool HasPrincipalChanged(ClaimsPrincipal? p1, ClaimsPrincipal? p2)
    {
        if (p1 == null || p2 == null)
            return true;

        if (p1.Identity?.IsAuthenticated == false && p2.Identity?.IsAuthenticated == false)
            return false;

        var c1 = p1.Claims.OrderBy(x => x.Type).ThenBy(x => x.Value).ToList();
        var c2 = p2.Claims.OrderBy(x => x.Type).ThenBy(x => x.Value).ToList();

        if (c1.Count != c2.Count)
            return true;

        for(int i = 0; i < c1.Count; i++)
        {
            if (c1[i].Type != c2[i].Type || c1[i].Value != c2[i].Value)
                return true;
        }

        return false;
    }

    /// <summary>
    /// Fetches the current user from a host provided Api.
    /// Most likely this uses some HttpClient to read the THostUser from a WebAPI.
    /// </summary>
    /// <returns>An instance of THostUser - if this is not null, the ClaimsPrincipal will be "authenticated"</returns>
    protected abstract Task<FetchUserResult<THostUser>> FetchUserAsync();

    /// <summary>
    /// The expiry time of the current user - if set, it will reduce the polling of FetchUserAsync()
    /// </summary>
    /// <returns>An absolute expiry time of the user, or null if unknown.</returns>
    protected abstract DateTimeOffset? GetAbsoluteExpiry(THostUser user);

    /// <summary>
    /// This functions transforms the THostUser into claims.
    /// </summary>
    /// <returns>A list of claims, that will be included in the ClaimsPrincipal object.</returns>
    protected abstract IEnumerable<Claim>? GetClaimsFromUser(THostUser user);
}

public record FetchUserResult<T>(
    bool IsAuthenticated,
    T? User);

