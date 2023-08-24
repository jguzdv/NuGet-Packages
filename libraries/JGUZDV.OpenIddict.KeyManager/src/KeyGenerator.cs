using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace JGUZDV.OpenIddict.KeyManager;

public class KeyGenerator
{
    private readonly IOptions<KeyManagerOptions> _options;
    private readonly ILogger<KeyGenerator> _logger;

    public KeyGenerator(
        IOptions<KeyManagerOptions> options,
        ILogger<KeyGenerator> logger)
    {
        _options = options;
        _logger = logger;
    }
}
