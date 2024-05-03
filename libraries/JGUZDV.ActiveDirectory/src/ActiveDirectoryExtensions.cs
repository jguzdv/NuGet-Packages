using System.Runtime.Versioning;

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
            services.AddSingleton<IPropertyValueReader, PropertyValueReader>();
            if (configure != null)
            {
                services.Configure(configure);
            }

            return services;
        }
    }
}
