using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace JGUZDV.OpenIddict.KeyManager;

public class KeyManagerService : IHostedService
{
    private readonly X509KeyStore _keyStore;
    private readonly X509CertificateKeyGenerator _keyGenerator;
    private readonly IConfigurationRoot _config;
    private readonly IOptions<KeyManagerOptions> _options;
    private readonly ILogger<KeyManagerService> _logger;
    
    private Timer? _timer;

    public KeyManagerService(
        X509KeyStore keyStore,
        X509CertificateKeyGenerator keyGenerator,
        TimeProvider timeProvider,
        IConfigurationRoot config,
        IOptions<KeyManagerOptions> options, 
        ILogger<KeyManagerService> logger)
    {
        _keyStore = keyStore;
        _keyGenerator = keyGenerator;
        _config = config;
        _options = options;
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _timer = new Timer(ExecuteTimer, null,
            TimeSpan.Zero, TimeSpan.FromMinutes(15));

        return EnsureUsableKeysAsync();
    }

    private async Task EnsureUsableKeysAsync()
    {
        foreach(var keyUsage in new[] { KeyUsage.Signature, KeyUsage.Encryption })
        {
            var keys = _keyStore.GetKeysAsync(keyUsage, forceCacheReload: true);
            
            if (keys.Any() == false)
                await _keyGenerator.CreateKey();
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_timer != null)
        {
            _timer.Change(Timeout.Infinite, 0);
            await _timer.DisposeAsync();
        }
    }

    private async void ExecuteTimer(object? state)
    {
        await ExecuteKeyManagement();
    }

    private async Task ExecuteKeyManagement()
    {
        _logger.LogInformation("Starting automatic key generation.");
        

        KeyManagerState keyState = await EnsureUsableKeyAsync(keyFiles);

        keyState &= await CreateNewKeys(keyFiles);
        keyState &= await CleanupOldKeys(keyFiles);

        if(keyState.HasFlag(KeyManagerState.CreatedKeys) || keyState.HasFlag(KeyManagerState.FoundAlternateKey))
            _config.Reload();
    }
}
