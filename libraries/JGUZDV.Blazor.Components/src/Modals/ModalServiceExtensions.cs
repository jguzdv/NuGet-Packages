using JGUZDV.Blazor.Components.Modals;

namespace Microsoft.Extensions.DependencyInjection;

public static class ModalServiceExtensions
{
    public static IServiceCollection AddModals(this IServiceCollection services)
        => services.AddSingleton<ModalService>();
}
