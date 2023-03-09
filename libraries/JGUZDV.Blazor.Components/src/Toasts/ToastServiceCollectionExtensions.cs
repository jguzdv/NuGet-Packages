using Microsoft.Extensions.DependencyInjection;
using JGUZDV.Blazor.Components.Toasts;

namespace JGUZDV.Blazor.Components
{
    public static class ToastServiceCollectionExtensions
    {
        public static IServiceCollection AddToasts(this IServiceCollection services)
        {
            services.AddSingleton<IToastService, ToastService>();

            return services;
        }
    }
}
