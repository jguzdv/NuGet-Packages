using JGUZDV.AspNetCore.Authentication.Cookies;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Caching.Distributed;

namespace Microsoft.Extensions.DependencyInjection;

public static class DistributedCacheTicketStoreExtensions
{
    public static AuthenticationBuilder AddCookieDistributedTicketStore(this AuthenticationBuilder builder,
        string cookieSchemaName = CookieAuthenticationDefaults.AuthenticationScheme)
    {
        builder.Services.AddOptions<CookieAuthenticationOptions>(cookieSchemaName)
            .PostConfigure<IDistributedCache>((opt, distributedCache) => {
                opt.SessionStore = new DistributedCacheTicketStore(distributedCache, opt.TicketDataFormat);
            });

        return builder;
    }
}
