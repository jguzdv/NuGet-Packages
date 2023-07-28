using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Caching.Distributed;

namespace JGUZDV.AspNetCore.Authentication.Cookies;

public class DistributedCacheTicketStore : ITicketStore
{
    private readonly string _keyPrefix = $"{nameof(DistributedCacheTicketStore)}.";

    private readonly IDistributedCache _distributedCache;
    private readonly ISecureDataFormat<AuthenticationTicket> _ticketDataFormat;

    public DistributedCacheTicketStore(IDistributedCache distributedCache,
        ISecureDataFormat<AuthenticationTicket> ticketDataFormat)
    {
        _distributedCache = distributedCache;
        _ticketDataFormat = ticketDataFormat;
    }

    public async Task RemoveAsync(string key)
    {
        await _distributedCache.RemoveAsync(key);
    }

    public async Task RenewAsync(string key, AuthenticationTicket ticket)
    {
        var entryOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpiration = ticket.Properties.ExpiresUtc
        };

        var protectedData = _ticketDataFormat.Protect(ticket);
        await _distributedCache.SetStringAsync(key, protectedData, entryOptions);
    }

    public async Task<AuthenticationTicket?> RetrieveAsync(string key)
    {
        var protectedData = await _distributedCache.GetStringAsync(key);
        if (protectedData == null)
            return null;

        return _ticketDataFormat.Unprotect(protectedData);
    }

    public async Task<string> StoreAsync(AuthenticationTicket ticket)
    {
        var key = $"{_keyPrefix}:{Guid.NewGuid()}";
        await RenewAsync(key, ticket);

        return key;
    }
}
