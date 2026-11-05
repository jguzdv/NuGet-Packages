using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Caching.Distributed;

namespace JGUZDV.AspNetCore.Authentication.Cookies;

/// <summary>
/// Provides a way to store authentication tickets in a distributed cache, allowing for scalable and persistent session management across multiple instances of an application.
/// </summary>
public class DistributedCacheTicketStore : ITicketStore
{
    private readonly string _keyPrefix = nameof(DistributedCacheTicketStore);

    private readonly IDistributedCache _distributedCache;
    private readonly ISecureDataFormat<AuthenticationTicket> _ticketDataFormat;

    /// <summary>
    /// Initializes a new instance of the <see cref="DistributedCacheTicketStore"/> class.
    /// </summary>
    /// <param name="distributedCache">The distributed cache to store authentication tickets.</param>
    /// <param name="ticketDataFormat">The data format used to protect and unprotect authentication tickets.</param>
    public DistributedCacheTicketStore(IDistributedCache distributedCache,
        ISecureDataFormat<AuthenticationTicket> ticketDataFormat)
    {
        _distributedCache = distributedCache;
        _ticketDataFormat = ticketDataFormat;
    }

    /// <inheritdoc />
    public async Task RemoveAsync(string key)
    {
        await _distributedCache.RemoveAsync(key);
    }

    /// <inheritdoc />
    public async Task RenewAsync(string key, AuthenticationTicket ticket)
    {
        var entryOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpiration = ticket.Properties.ExpiresUtc
        };

        var protectedData = _ticketDataFormat.Protect(ticket);
        await _distributedCache.SetStringAsync(key, protectedData, entryOptions);
    }

    /// <inheritdoc />
    public async Task<AuthenticationTicket?> RetrieveAsync(string key)
    {
        var protectedData = await _distributedCache.GetStringAsync(key);
        if (protectedData == null)
            return null;

        return _ticketDataFormat.Unprotect(protectedData);
    }
    
    /// <inheritdoc />
    public async Task<string> StoreAsync(AuthenticationTicket ticket)
    {
        var key = $"{_keyPrefix}:{Guid.NewGuid()}";
        await RenewAsync(key, ticket);

        return key;
    }
}
