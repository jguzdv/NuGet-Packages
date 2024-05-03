using System.Runtime.Versioning;

using JGUZDV.ActiveDirectory;
using JGUZDV.ActiveDirectory.ClaimProvider;
using JGUZDV.ActiveDirectory.ClaimProvider.Configuration;
using JGUZDV.ActiveDirectory.ClaimProvider.PropertyConverters;
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
            services.AddConverters();

            return services;
        }

        [SupportedOSPlatform("windows")]
        public static IServiceCollection AddConverters(this IServiceCollection services)
        {
            services.AddScoped<IPropertyConverterFactory, PropertyConverterFactory>();

            services.TryAddEnumerable(ServiceDescriptor.Singleton<IPropertyConverter, ByteConverter>());
            services.TryAddEnumerable(ServiceDescriptor.Singleton<IPropertyConverter, StringConverter>());
            services.TryAddEnumerable(ServiceDescriptor.Singleton<IPropertyConverter, IntConverter>());
            services.TryAddEnumerable(ServiceDescriptor.Singleton<IPropertyConverter, LongConverter>());
            services.TryAddEnumerable(ServiceDescriptor.Singleton<IPropertyConverter, DateTimeConverter>());

            return services;
        }
    }
}
