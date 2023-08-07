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

            services.TryAddEnumerable(ServiceDescriptor.Singleton<IPropertyConverter, ByteBase64Converter>());
            services.TryAddEnumerable(ServiceDescriptor.Singleton<IPropertyConverter, ByteGuidConverter>());
            services.TryAddEnumerable(ServiceDescriptor.Singleton<IPropertyConverter, ByteSIDConverter>());
            services.TryAddEnumerable(ServiceDescriptor.Singleton<IPropertyConverter, LowerStringConverter>());
            services.TryAddEnumerable(ServiceDescriptor.Singleton<IPropertyConverter, StringConverter>());
            services.TryAddEnumerable(ServiceDescriptor.Singleton<IPropertyConverter, UpperStringConverter>());
            services.TryAddEnumerable(ServiceDescriptor.Singleton<IPropertyConverter, IntConverter>());

            return services;
        }
    }
}
