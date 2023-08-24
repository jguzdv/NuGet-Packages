using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace JGUZDV.OpenIddict.KeyManager;

public class KeyStore
{
    private readonly IOptions<KeyManagerOptions> _options;
    private readonly ILogger<KeyStore> _logger;

    public KeyStore(
        IOptions<KeyManagerOptions> options,
        ILogger<KeyStore> logger)
    {
        _options = options;
        _logger = logger;
    }
}
