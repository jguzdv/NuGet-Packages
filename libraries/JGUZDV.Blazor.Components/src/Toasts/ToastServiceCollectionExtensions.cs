using Microsoft.Extensions.DependencyInjection;
using JGUZDV.Blazor.Components.Toasts;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace JGUZDV.Blazor.Components
{
    public static class ToastServiceCollectionExtensions
    {
        public static IServiceCollection AddToasts(this IServiceCollection services)
        {
            services.TryAddScoped<IToastService, ToastService>();

            return services;
        }
    }
}
