using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace JGUZDV.OpenIddict.KeyManager;

// TODO: The logic that decides, if a config reload is necessary is .. flawed.
public class KeyManagerService : IHostedService
{
    private readonly X509KeyStore _keyStore;
    private readonly X509CertificateKeyGenerator _keyGenerator;
    private readonly TimeProvider _timeProvider;
    private readonly IConfigurationRoot _config;
    private readonly IOptions<KeyManagerOptions> _options;
    private readonly ILogger<KeyManagerService> _logger;

    private readonly CancellationTokenSource _cts = new();

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
        _timeProvider = timeProvider;
        _config = config;
        _options = options;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken ct)
    {
        await EnsureUsableKeysAsync(ct);

        if (!ct.IsCancellationRequested)
        {
            _timer = new Timer(ExecuteTimer, null,
                _options.Value.KeyReloadInterval, _options.Value.KeyReloadInterval);
        }

        _config.Reload();
    }

    public async Task StopAsync(CancellationToken ct)
    {
        _cts.Cancel();

        if (_timer != null)
        {
            _timer.Change(Timeout.Infinite, 0);
            await _timer.DisposeAsync();
        }
    }


    private void ExecuteTimer(object? state)
        => _ = ExecuteKeyManagement(_cts.Token);


    private async Task ExecuteKeyManagement(CancellationToken ct)
    {
        await _keyStore.ReloadKeysAsync(ct);
        
        if(_options.Value.DisableKeyGeneration)
        {
            _config.Reload();
            return;
        }

        _logger.LogInformation("Starting automatic key generation.");
        await Task.WhenAll(
            ExecuteKeyManagement(KeyUsage.Signature, ct),
            ExecuteKeyManagement(KeyUsage.Encryption, ct)
        );

        _config.Reload();
    }

    private async Task ExecuteKeyManagement(KeyUsage keyUsage, CancellationToken ct)
    {
        var keys = await _keyStore.GetKeysAsync(keyUsage, true, ct);
        if (_options.Value.DisableKeyGeneration)
            return;

        if(!keys.Any())
        {
            await EnsureUsableKeysAsync(keyUsage, ct);
            return;
        }

        var utcNow = _timeProvider.GetUtcNow();
        await PrepareNextKey();
        await PurgeExpiredKeysAsync();
        _config.Reload();

        async Task PrepareNextKey()
        {
            var maxNotAfter = keys
                .Where(x => x.Certificate.NotBefore < utcNow && x.Certificate.NotAfter > utcNow)
                .Max(x => x.Certificate.NotAfter);

            var prepareNextKeyFrom = maxNotAfter.Add(_options.Value.MaxKeyAge / -1.5);

            if (prepareNextKeyFrom <= utcNow)
                return;

            var futureKey = keys.Where(x =>
                x.Certificate.NotAfter > utcNow &&
                x.Certificate.NotBefore < maxNotAfter &&
                x.Certificate.NotAfter > maxNotAfter)
                .FirstOrDefault();

            if (futureKey is not null)
                return;

            var nextKey = _keyGenerator.CreateKey(keyUsage, maxNotAfter.AddDays(-1), maxNotAfter.Add(_options.Value.MaxKeyAge));
            await _keyStore.SaveKeyAsync(keyUsage, nextKey, ct);
        }

        async Task PurgeExpiredKeysAsync()
        {
            await _keyStore.PurgeExpiredKeysAsync(keyUsage, utcNow + _options.Value.KeyRetention, ct);
        }
    }
    


    private async Task EnsureUsableKeysAsync(CancellationToken ct)
    {
        await _keyStore.ReloadKeysAsync(ct);

        await Task.WhenAll(
            EnsureUsableKeysAsync(KeyUsage.Signature, ct),
            EnsureUsableKeysAsync(KeyUsage.Encryption, ct)
        );
    }

    private async Task EnsureUsableKeysAsync(KeyUsage usage, CancellationToken ct)
    {
        var utcNow = _timeProvider.GetUtcNow();
        while(!ct.IsCancellationRequested && await CheckIfKeyExists())
        {
            var didCreateKey = await TryCreateNewKey();
            if(!didCreateKey)
            {
                await Task.Delay(_options.Value.RetryDelay, ct);
            }
        } 

        async Task<bool> CheckIfKeyExists()
        {
            var keys = await _keyStore.GetKeysAsync(usage, true, ct);
            return keys.Any(x => x.Certificate.NotBefore < utcNow && x.Certificate.NotAfter > utcNow);
        }

        async Task<bool> TryCreateNewKey()
        {
            if (_options.Value.DisableKeyGeneration)
                return false;

            try
            {
                var createdKey = _keyGenerator.CreateKey(usage, utcNow.Date, utcNow.Date.Add(_options.Value.MaxKeyAge));
                await _keyStore.SaveKeyAsync(usage, createdKey, ct);

                return true;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Could not create new key.");
                return false;
            }
        }
    }
}
