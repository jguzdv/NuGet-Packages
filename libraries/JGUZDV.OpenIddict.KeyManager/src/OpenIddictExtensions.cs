using JGUZDV.OpenIddict.KeyManager;

namespace Microsoft.Extensions.DependencyInjection;

public static class OpenIddictKeyManagerExtensions
{
    public static IServiceCollection AddAutomaticKeyRollover(this IServiceCollection services, Action<KeyManagerOptions>? configureAction)
    {
        services.AddSingleton<KeyContainer>();

        services.AddSingleton<KeyManagerService>();
        services.AddHostedService(sp => sp.GetRequiredService<KeyManagerService>());

        services.ConfigureOptions<KeyManagerOptionsValidator>();
        services.ConfigureOptions<OpenIdDictServerConfiguration>();

        services.AddSingleton<X509CertificateKeyGenerator>();
        services.AddSingleton<X509KeyStore>();

        if(configureAction != null)
            services.Configure(configureAction);

        return services;
    }
}
