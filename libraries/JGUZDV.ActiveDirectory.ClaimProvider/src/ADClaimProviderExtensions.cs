using System.Runtime.Versioning;

using JGUZDV.ActiveDirectory;
using JGUZDV.ActiveDirectory.ClaimProvider;
using JGUZDV.ActiveDirectory.ClaimProvider.Configuration;
using JGUZDV.ActiveDirectory.Configuration;

using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ADClaimProviderExtensions
    {
        [SupportedOSPlatform("windows")]
        public static IServiceCollection AddActiveDirectoryClaimProvider(
            this IServiceCollection services, 
            Action<ActiveDirectoryOptions>? configure = null,
            Action<PropertyReaderOptions>? configurePropertyReader = null)
        {
            if(configure != null)
                services.Configure(configure);

            services.AddPropertyReader(configurePropertyReader);

            services.AddScoped<ADClaimProvider>();

            return services;
        }
    }
}
