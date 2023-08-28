using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using OpenIddict.Server;

namespace JGUZDV.OpenIddict.KeyManager;

public class KeyManagerConfiguration : IConfigureOptions<OpenIddictServerOptions>
{
    private readonly X509KeyStore _keyStore;
    private readonly ILogger<KeyManagerConfiguration> _logger;

    public KeyManagerConfiguration(
        X509KeyStore keyStore,
        ILogger<KeyManagerConfiguration> logger)
    {
        _keyStore = keyStore;
        _logger = logger;
    }

    public void Configure(OpenIddictServerOptions options)
    {
        
    }
}
