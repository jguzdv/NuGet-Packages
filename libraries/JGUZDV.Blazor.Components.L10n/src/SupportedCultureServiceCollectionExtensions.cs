using Microsoft.Extensions.DependencyInjection;

namespace JGUZDV.Blazor.Components.L10n
{
    /// <summary>
    /// Service extension.
    /// </summary>
    public static class SupportedCultureServiceCollectionExtensions
    {
        /// <summary>
        /// Method to add the service and interface to the project and to pass on the supported cultures.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="supportedCultures"></param>
        /// <returns></returns>
        public static IServiceCollection AddSupportedCultures(this IServiceCollection services, List<string> supportedCultures)
        {
            services.AddSingleton<ISupportedCultureService, SupportedCultureService>(x => new SupportedCultureService(supportedCultures));
            return services;
        }
    }
}
