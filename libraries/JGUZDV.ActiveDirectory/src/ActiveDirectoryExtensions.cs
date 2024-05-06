using System.Runtime.Versioning;

using JGUZDV.ActiveDirectory.Claims;

using Microsoft.Extensions.DependencyInjection;

namespace JGUZDV.ActiveDirectory
{
    /// <summary>
    /// Extension methods for adding Active Directory services to the DI container.
    /// </summary>
    [SupportedOSPlatform("windows")]
    public static class ActiveDirectoryExtensions
    {
        /// <summary>
        /// Adds the property reader to the DI container.
        /// </summary>
        public static IServiceCollection AddPropertyReader(this IServiceCollection services, Action<Configuration.PropertyReaderOptions>? configure = null)
        {
            if (configure != null)
            {
                services.Configure(configure);
            }
            services.AddSingleton<IPropertyReader, PropertyReader>();

            return services;
        }

        
        /// <summary>
        /// Adds the claim provider to the DI container.
        /// </summary>
        public static IServiceCollection AddClaimProvider(this IServiceCollection services, Action<Configuration.ClaimProviderOptions>? configure = null)
        {
            if (configure != null)
            {
                services.Configure(configure);
            }

            services.AddSingleton<IClaimProvider, ClaimProvider>();

            return services;
        }
    }
}
