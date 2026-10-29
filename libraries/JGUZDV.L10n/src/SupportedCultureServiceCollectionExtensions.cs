using Microsoft.Extensions.DependencyInjection;

namespace JGUZDV.L10n
{
    /// <summary>
    /// Handles the method to pass on the supported cultures.
    /// </summary>
    public static class SupportedCultureServiceCollectionExtensions
    {

        /// <summary>
        /// Adds the <see cref="ISupportedCultureService"/> using the default <see cref="ILanguageService"/>.
        /// </summary>
        /// <returns></returns>
        public static IServiceCollection AddDefaultSupportedCultures(this IServiceCollection services)
        {
            services.AddSingleton<ISupportedCultureService, SupportedCultureService>(x => new SupportedCultureService(x.GetRequiredService<ILanguageService>()));
            return services;
        }

        /// <summary>
        /// Adds the service and interface to the project and passes on the supported cultures.
        /// </summary>
        /// <returns></returns>
        public static IServiceCollection AddSupportedCultures(this IServiceCollection services, IEnumerable<string> supportedCultures)
        {
            services.AddSingleton<ISupportedCultureService, SupportedCultureService>(x => new SupportedCultureService(supportedCultures.ToList()));
            return services;
        }
    }
}
