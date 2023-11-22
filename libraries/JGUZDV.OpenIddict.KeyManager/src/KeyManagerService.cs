using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace JGUZDV.OpenIddict.KeyManager;

public class KeyManagerService : IHostedService
{
    private readonly X509CertificateKeyGenerator _keyGenerator;
    private readonly X509KeyStore _keyStore;
    private readonly KeyContainer _keyContainer;
    private readonly TimeProvider _timeProvider;
    private readonly IConfigurationRoot _config;
    private readonly IOptions<KeyManagerOptions> _options;
    private readonly ILogger<KeyManagerService> _logger;

    private readonly CancellationTokenSource _cts = new();

    private Timer? _timer;

    public KeyManagerService(
        X509CertificateKeyGenerator keyGenerator,
        X509KeyStore keyStore,
        KeyContainer keyContainer,
        TimeProvider timeProvider,
        IConfigurationRoot config,
        IOptions<KeyManagerOptions> options, 
        ILogger<KeyManagerService> logger)
    {
        _keyStore = keyStore;
        _keyGenerator = keyGenerator;
        _keyContainer = keyContainer;
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
        var keyInfos = await _keyStore.LoadKeysAsync(ct);
        
        if(!_options.Value.DisableKeyGeneration)
        {
            keyInfos = await EnsureUsableKeysAsync(keyInfos, ct);
            await ExecuteKeyManagement(keyInfos, ct);
        }

        _keyContainer.ReplaceAllKeys(keyInfos);
        _config.Reload();
    }

    private async Task ExecuteKeyManagement(List<KeyInfo> keyInfos, CancellationToken ct)
    {
        var utcNow = _timeProvider.GetUtcNow();
        await PrepareNextKeysAsync(keyInfos);
        await PurgeExpiredKeysAsync();


        async Task PrepareNextKeysAsync(List<KeyInfo> keyInfos)
        {
            
            var usages = new[] { KeyUsage.Signature, KeyUsage.Encryption };
            foreach (var usage in usages)
            {
                var securityKeys = keyInfos.Where(x => x.KeyUsage == usage)
                    .Select(x => x.SecurityKey);

                var maxNotAfter = securityKeys
                    .Where(x => x.Certificate.NotBefore < utcNow && x.Certificate.NotAfter > utcNow)
                    .Max(x => x.Certificate.NotAfter);

                // The threshold date will be _options.Value.ThresholdFactor * MaxKeyAge before any valid the maximum of NotAfter of the current keys 
                var thresholdDate = maxNotAfter.Add(-1* Math.Abs(_options.Value.ThresholdFactor) * _options.Value.MaxKeyAge);

                if (utcNow <= thresholdDate)
                {
                    _logger.LogDebug("Threshold date ({thresholdDate} for certificate creation has not been reached.", thresholdDate);
                    continue;
                }

                var nextKey = await PrepareNextKeyAsync(usage, maxNotAfter, securityKeys);
                if(nextKey != null)
                    keyInfos.Add(new(usage, nextKey));
            }
        }

        async Task<X509SecurityKey?> PrepareNextKeyAsync(KeyUsage usage, DateTimeOffset refDate,
            IEnumerable<X509SecurityKey> securityKeys)
        {
            var futureKey = securityKeys
                .Where(x =>
                    x.Certificate.NotBefore < refDate &&
                    x.Certificate.NotAfter > refDate.AddDays(1)
                ).FirstOrDefault();

            // This is necessary to check, since we might already have a future key
            if (futureKey is not null)
            {
                _logger.LogDebug("A {usage} key that is valid after {refDate} already exists.", usage, refDate);
                return null;
            }

            var nextKey = _keyGenerator.CreateKey(usage, refDate.AddDays(-1), refDate.Add(_options.Value.MaxKeyAge));
            await _keyStore.SaveKeyAsync(usage, nextKey, ct);

            return nextKey;
        }

        async Task PurgeExpiredKeysAsync()
        {
            await _keyStore.PurgeExpiredKeys(utcNow + _options.Value.KeyRetention, ct);
        }
    }
    


    public async Task EnsureUsableKeysAsync(CancellationToken ct)
    {
        var keyInfos = await _keyStore.LoadKeysAsync();
        keyInfos = await EnsureUsableKeysAsync(keyInfos, ct);

        _keyContainer.ReplaceAllKeys(keyInfos);
    }

    /// <returns>True, if it created keys.</returns>
    private async Task<List<KeyInfo>> EnsureUsableKeysAsync(IEnumerable<KeyInfo> keyInfos, CancellationToken ct)
    {
        var utcNow = _timeProvider.GetUtcNow();
        var localKeyInfos = keyInfos.ToList();

        while(!ct.IsCancellationRequested && !KeysExist(localKeyInfos))
        {
            if (_options.Value.DisableKeyGeneration)
            {
                _logger.LogInformation("No key has been found and key generation is disabled. Waiting for key to be created.");
            }
            else
            {
                if (await TryCreateNewKeys(localKeyInfos, ct))
                    break;
            }
            
            // If we could not find or create a new key for whatever reason.
            localKeyInfos = await DelayAndReload(ct);
            await Task.Delay(_options.Value.RetryDelay, ct);
        }

        return localKeyInfos;



        bool KeysExist(List<KeyInfo> keyInfos)
            => KeyForUsageExists(keyInfos, KeyUsage.Signature)
                && KeyForUsageExists(keyInfos, KeyUsage.Encryption);

        bool KeyForUsageExists(List<KeyInfo> keyInfos, KeyUsage usage)
            => keyInfos.Any(x => x.KeyUsage == usage && IsKeyUsable(x.SecurityKey, utcNow));

        bool IsKeyUsable(X509SecurityKey securityKey, DateTimeOffset refDate) 
            => securityKey.Certificate.NotBefore < refDate && securityKey.Certificate.NotAfter > refDate;

        async Task<List<KeyInfo>> DelayAndReload(CancellationToken ct)
        {
            await Task.Delay(_options.Value.RetryDelay, ct);
            return await _keyStore.LoadKeysAsync(ct);
        }

        async Task<bool> TryCreateNewKeys(List<KeyInfo> keyInfos, CancellationToken ct)
        {
            var usages = new[] { KeyUsage.Signature, KeyUsage.Encryption };

            var result = true;
            foreach (var usage in usages)
            {
                if (KeyForUsageExists(keyInfos, usage))
                    continue;

                try
                {
                    var createdKey = _keyGenerator.CreateKey(usage, utcNow.Date, utcNow.Date.Add(_options.Value.MaxKeyAge));
                    await _keyStore.SaveKeyAsync(usage, createdKey, ct);
                    keyInfos.Add(new(usage, createdKey));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Could not create new key.");
                    result = false;
                }
            }

            return result;
        }
    }
}
