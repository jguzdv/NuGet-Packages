using Microsoft.Extensions.DependencyInjection;
using JGUZDV.Blazor.Components.Toasts;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace JGUZDV.Blazor.Components
{
    /// <summary>
    /// Extensions for adding toast services to the service collection.
    /// </summary>
    [Obsolete("Use ToastMessages component instead of this.")]
    public static class ToastServiceCollectionExtensions
    {
        [Obsolete("Use ToastMessages component instead of this.")]
        public static IServiceCollection AddToasts(this IServiceCollection services)
        {
            services.TryAddScoped<IToastService, ToastService>();

            return services;
        }
    }
}
