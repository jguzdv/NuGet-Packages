using Microsoft.Extensions.DependencyInjection;

namespace JGUZDV.Blazor.Components.L10n
{
    public static class SupportedCultureServiceCollectionExtensions
    {
        public static IServiceCollection AddSupportedCultures(this IServiceCollection services)
        {
            services.AddSingleton<ISupportedCultureService, SupportedCultureService>();

            return services;
        }
    }
}
