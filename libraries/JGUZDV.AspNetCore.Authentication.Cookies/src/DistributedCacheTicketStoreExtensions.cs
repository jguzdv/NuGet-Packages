using JGUZDV.AspNetCore.Authentication.Cookies;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Caching.Distributed;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Provides extension methods for configuring the <see cref="DistributedCacheTicketStore"/> in the authentication pipeline.
/// </summary>
public static class DistributedCacheTicketStoreExtensions
{
    /// <summary>
    /// Adds the <see cref="DistributedCacheTicketStore"/> to the authentication pipeline, allowing for the storage of authentication tickets in a distributed cache instead of cookies.
    /// </summary>
    public static AuthenticationBuilder AddCookieDistributedTicketStore(this AuthenticationBuilder builder,
        string cookieSchemaName = CookieAuthenticationDefaults.AuthenticationScheme)
    {
        builder.Services.AddOptions<CookieAuthenticationOptions>(cookieSchemaName)
            .PostConfigure<IDistributedCache>((opt, distributedCache) 
                => opt.SessionStore = new DistributedCacheTicketStore(distributedCache, opt.TicketDataFormat)
            );

        return builder;
    }
}
