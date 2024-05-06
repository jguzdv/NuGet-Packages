using JGUZDV.OpenIddict.KeyManager;
using JGUZDV.OpenIddict.KeyManager.Configuration;
using JGUZDV.OpenIddict.KeyManager.RSA;
using JGUZDV.OpenIddict.KeyManager.X509;

namespace Microsoft.Extensions.DependencyInjection;

public static class OpenIddictKeyManagerExtensions
{
    public static IServiceCollection AddAutomaticKeyRollover(
        this IServiceCollection services,
        KeyType keyType,
        Action<KeyManagerOptions>? configureAction)
    {
        services.AddSingleton<KeyContainer>();

        services.AddSingleton<KeyManagerService>();
        services.AddHostedService(sp => sp.GetRequiredService<KeyManagerService>());

        services.ConfigureOptions<KeyManagerOptionsValidator>();
        services.ConfigureOptions<OpenIdDictServerConfiguration>();

        if(keyType == KeyType.RSA)
        {
            services.AddSingleton<IKeyGenerator, RsaKeyGenerator>();
            services.AddSingleton<IKeyStore, RsaKeyStore>();
        }
        else
        {
            services.AddSingleton<IKeyGenerator, X509CertificateKeyGenerator>();
            services.AddSingleton<IKeyStore, X509KeyStore>();
        }

        if(configureAction != null)
            services.Configure(configureAction);

        return services;
    }

    public enum KeyType
    {
        RSA,
        X509
    }
}
