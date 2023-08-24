using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace JGUZDV.OpenIddict.KeyManager;

public class KeyManagerService : IHostedService
{
    private readonly KeyStore _keyStore;
    private readonly KeyGenerator _keyGenerator;
    private readonly IOptions<KeyManagerOptions> _options;
    private readonly ILogger<KeyManagerService> _logger;
    private Timer? _timer;

    public KeyManagerService(
        KeyStore keyStore,
        KeyGenerator keyGenerator,
        IOptions<KeyManagerOptions> options, 
        ILogger<KeyManagerService> logger)
    {
        _keyStore = keyStore;
        _keyGenerator = keyGenerator;
        _options = options;
        _logger = logger;
        
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _timer = new Timer(ExecuteTimer, null,
            TimeSpan.Zero, TimeSpan.FromMinutes(15));


        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_timer != null)
        {
            _timer.Change(Timeout.Infinite, 0);
            await _timer.DisposeAsync();
        }
    }

    private async void ExecuteTimer(object state)
    {
        await ExecuteKeyManagement();
    }

    private async Task ExecuteKeyManagement()
    {
        _logger.LogInformation("Starting automatic key generation.");
        var keyFiles = (await _keyLocator.FindKeyFilesAsync()).ToList();

        await EnsureSignatureKeyAsync(keyFiles);

        await CreateNewKeys(keyFiles);
        await CleanupOldKeys(keyFiles);
    }
}
