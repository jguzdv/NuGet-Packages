using Microsoft.Extensions.DependencyInjection;

namespace JGUZDV.Blazor.Components.NavigatorSwitch;

public static class NavigatorSwitchServiceCollectionExtensions
{
    public static string DefaultNavigatorBaseAddress = "https://start.uni-mainz.de";

    public static IServiceCollection AddNavigatorSwitchServices(
        this IServiceCollection services,
        string? baseAddress)
    {
        services.AddHttpClient<NavigatorSwitchHttpClient>(client =>
        {
            client.BaseAddress = new Uri(baseAddress ?? DefaultNavigatorBaseAddress);
        });

        services.AddSingleton<NavigatorSwitchService>();

        return services;
    }
}
