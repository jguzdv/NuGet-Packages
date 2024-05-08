using JGUZDV.OpenIddict.KeyManager.Configuration;
using JGUZDV.OpenIddict.KeyManager.Model;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace JGUZDV.OpenIddict.KeyManager;

public class KeyManagerService : IHostedService
{
    private readonly IKeyGenerator _keyGenerator;
    private readonly IKeyStore _keyStore;
    private readonly KeyContainer _keyContainer;
    private readonly TimeProvider _timeProvider;
    private readonly IConfigurationRoot _config;
    private readonly IOptions<KeyManagerOptions> _options;
    private readonly ILogger<KeyManagerService> _logger;

    private readonly CancellationTokenSource _cts = new();

    private Timer? _timer;

    public KeyManagerService(
        IKeyGenerator keyGenerator,
        IKeyStore keyStore,
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

    /// <inheritdoc />
    public async Task StartAsync(CancellationToken ct)
    {
        await EnsureAndLoadUsableKeysAsync(ct);

        if (!ct.IsCancellationRequested)
        {
            _timer = new Timer(ExecuteTimer, null,
                _options.Value.KeyReloadInterval, _options.Value.KeyReloadInterval);
        }
    }

    /// <inheritdoc />
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
        => _ = ReloadKeysAndRunManagement(_cts.Token);



    private async Task ReloadKeysAndRunManagement(CancellationToken ct)
    {
        var signatureKeys = await _keyStore.LoadKeysAsync(KeyUsage.Signature, ct);
        var encryptionKeys = await _keyStore.LoadKeysAsync(KeyUsage.Encryption, ct);

        if (!_options.Value.DisableKeyGeneration)
        {
            signatureKeys = await ExecuteKeyManagement(signatureKeys, KeyUsage.Signature, ct);
            encryptionKeys = await ExecuteKeyManagement(encryptionKeys, KeyUsage.Encryption, ct);
        }

        _keyContainer.ReplaceAllKeys(signatureKeys, encryptionKeys);
        _config.Reload();
    }

    private async Task<List<KeyInfo>> ExecuteKeyManagement(List<KeyInfo> keyInfos, KeyUsage keyUsage, CancellationToken ct)
    {
        var utcNow = _timeProvider.GetUtcNow();

        keyInfos = await EnsureUsableKeysAsync(keyInfos, keyUsage, ct);
        await PrepareNextKeysAsync(keyInfos, keyUsage, ct);
        await _keyStore.PurgeExpiredKeys(keyUsage, utcNow - _options.Value.KeyRetention, ct);

        return keyInfos;


        // Local functions

        async Task PrepareNextKeysAsync(List<KeyInfo> keyInfos, KeyUsage keyUsage, CancellationToken ct)
        {
            var maxNotAfter = keyInfos
                .Where(x => x.NotBefore < utcNow && x.NotAfter > utcNow)
                .Max(x => x.NotAfter);

            // The threshold date will be _options.Value.ThresholdFactor * MaxKeyAge before any valid the maximum of NotAfter of the current keys,
            // e.g. if a key is valid for 30 days and the threshold factor is 0.5, the threshold date will be 15 days before the key expires.
            var thresholdDate = maxNotAfter.Add(-1 * Math.Abs(_options.Value.ThresholdFactor) * _options.Value.MaxKeyAge);

            if (utcNow <= thresholdDate)
            {
                _logger.LogDebug("Threshold date ({thresholdDate} for certificate creation has not been reached.", thresholdDate);
                return;
            }

            var nextKey = await CreateNextKeyIfMissing(keyInfos, keyUsage, maxNotAfter, ct);
            if (nextKey != null)
                keyInfos.Add(nextKey);
        }

        async Task<KeyInfo?> CreateNextKeyIfMissing(IEnumerable<KeyInfo> securityKeys, KeyUsage usage,
            DateTimeOffset refDate, CancellationToken ct)
        {
            var futureKey = securityKeys
                .Where(x =>
                    x.NotBefore < refDate &&
                    x.NotAfter > refDate.AddDays(1)
                ).FirstOrDefault();

            // This is necessary to check, since we might already have a key for future use.
            if (futureKey is not null)
            {
                _logger.LogDebug("A {usage} key that is valid after {refDate} already exists.", usage, refDate);
                return null;
            }

            var notBefore = refDate.AddDays(-1);
            var notAfter = refDate.Add(_options.Value.MaxKeyAge);

            var nextKey = _keyGenerator.CreateKey(usage, notBefore, notAfter);
            var nextKeyInfo = new KeyInfo(usage, nextKey, notBefore, notAfter);
            await _keyStore.SaveKeyAsync(nextKeyInfo, ct);

            return nextKeyInfo;
        }
    }



    private async Task EnsureAndLoadUsableKeysAsync(CancellationToken ct)
    {
        var signatureKeys = await _keyStore.LoadKeysAsync(KeyUsage.Signature, ct);
        var encryptionKeys = await _keyStore.LoadKeysAsync(KeyUsage.Encryption, ct);

        signatureKeys = await EnsureUsableKeysAsync(signatureKeys, KeyUsage.Signature, ct);
        encryptionKeys = await EnsureUsableKeysAsync(encryptionKeys, KeyUsage.Encryption, ct);

        _keyContainer.ReplaceAllKeys(signatureKeys, encryptionKeys);
        _config.Reload();
    }


    private async Task<List<KeyInfo>> EnsureUsableKeysAsync(List<KeyInfo> keyInfos, KeyUsage keyUsage, CancellationToken ct)
    {
        var utcNow = _timeProvider.GetUtcNow();

        while (!ct.IsCancellationRequested && !keyInfos.Any(x => IsKeyUsable(x, utcNow)))
        {
            if(await TryCreateNewKey(keyInfos, keyUsage, utcNow, ct))
            {
                break;
            }    

            // If we could not find or create a new key for whatever reason.
            keyInfos = await DelayAndReload(ct);
            await Task.Delay(_options.Value.RetryDelay, ct);
        }

        return keyInfos;



        bool IsKeyUsable(KeyInfo keyInfo, DateTimeOffset refDate)
            => keyInfo.NotBefore < refDate && keyInfo.NotAfter > refDate;

        async Task<List<KeyInfo>> DelayAndReload(CancellationToken ct)
        {
            await Task.Delay(_options.Value.RetryDelay, ct);
            return await _keyStore.LoadKeysAsync(keyUsage, ct);
        }


        async Task<bool> TryCreateNewKey(List<KeyInfo> keyInfos, KeyUsage keyUsage, DateTimeOffset refDate, CancellationToken ct)
        {
            if (_options.Value.DisableKeyGeneration)
            {
                _logger.LogInformation("No key has been found and key generation is disabled. Waiting for key to be created.");
                return false;
            }

            try
            {
                var notBefore = refDate.Date;
                var notAfter = refDate.Date.Add(_options.Value.MaxKeyAge);

                var createdKey = _keyGenerator.CreateKey(keyUsage, notBefore, notAfter);
                var createdKeyInfo = new KeyInfo(keyUsage, createdKey, notBefore, notAfter);
                await _keyStore.SaveKeyAsync(createdKeyInfo, ct);
                keyInfos.Add(createdKeyInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not create new key.");
                return false;
            }
            
            return true;
        }
    }
}
