using System.Runtime.Versioning;

using JGUZDV.ActiveDirectory.ClaimProvider;
using JGUZDV.ActiveDirectory.ClaimProvider.Configuration;
using JGUZDV.ActiveDirectory.ClaimProvider.PropertyConverters;

using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ADClaimProviderExtensions
    {
        [SupportedOSPlatform("windows")]
        public static IServiceCollection AddActiveDirectoryClaimProvider(
            this IServiceCollection services, Action<ActiveDirectoryOptions>? configure = null)
        {
            if(configure != null)
                services.Configure(configure);

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
