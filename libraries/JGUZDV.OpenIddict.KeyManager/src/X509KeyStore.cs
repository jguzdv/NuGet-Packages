using System.Security.Cryptography.X509Certificates;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace JGUZDV.OpenIddict.KeyManager;

public class X509KeyStore
{
    private readonly TimeProvider _timeProvider;
    private readonly IOptions<KeyManagerOptions> _options;
    private readonly ILogger<X509KeyStore> _logger;

    private readonly Dictionary<KeyUsage, List<X509SecurityKey>> _securityKeys;

    internal event EventHandler? KeyStoreUpdated;

    public X509KeyStore(
        TimeProvider timeProvider,
        IOptions<KeyManagerOptions> options,
        ILogger<X509KeyStore> logger)
    {
        _timeProvider = timeProvider;
        _options = options;
        _logger = logger;

        _securityKeys = new Dictionary<KeyUsage, List<X509SecurityKey>>
        {
            { KeyUsage.Signature, new () },
            { KeyUsage.Encryption, new () }
        };
    }


    private void OnKeyStoreUpdated() => KeyStoreUpdated?.Invoke(this, EventArgs.Empty);


    internal async Task<List<X509SecurityKey>> GetKeysAsync(KeyUsage keyUsage, bool forceCacheReload,
        CancellationToken ct)
    {
        if(forceCacheReload || !_securityKeys[keyUsage].Any())
        {
            try
            {
                _securityKeys[keyUsage] = await LoadKeysAsync(keyUsage, ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Loading keys was not possible.");
            }
        }

        return _securityKeys[keyUsage];
    }

    internal List<X509SecurityKey> GetKeys(KeyUsage keyUsage) => _securityKeys[keyUsage];



    private async Task<List<X509SecurityKey>> LoadKeysAsync(KeyUsage keyUsage, CancellationToken ct)
    {
        var keyStorePath = _options.Value.KeyStorePath;
        var pattern = $"*.{GetFileExtension(keyUsage)}";

        var utcNow = _timeProvider.GetUtcNow();
        var securityKeys = new List<X509SecurityKey>();
        foreach(var fileName in Directory.EnumerateFiles(keyStorePath, pattern))
        {
            var key = await LoadKeyAsync(fileName, ct);
            if (key.Certificate.NotAfter > utcNow)
                securityKeys.Add(key);
        }

        return securityKeys;
    }


    private static async Task<X509SecurityKey> LoadKeyAsync(string fileName, CancellationToken ct)
    {
        var certificateBytes = await File.ReadAllBytesAsync(fileName, ct);
        var certificate = new X509Certificate2(certificateBytes);
        return new X509SecurityKey(certificate);
    }



    internal async Task SaveKeyAsync(KeyUsage keyUsage, X509SecurityKey securityKey, CancellationToken ct)
    {
        var fileName = Path.Combine(
            _options.Value.KeyStorePath,
            Path.ChangeExtension(securityKey.Certificate.Thumbprint, GetFileExtension(keyUsage))
        );

        var certificateBytes = securityKey.Certificate.Export(X509ContentType.Pkcs12, string.Empty);
        await File.WriteAllBytesAsync(fileName, certificateBytes, ct);

        _securityKeys[keyUsage].Add(securityKey);
        OnKeyStoreUpdated();
    }



    internal async Task PurgeExpiredKeysAsync(KeyUsage keyUsage, DateTimeOffset refDate, CancellationToken ct)
    {
        var keyStorePath = _options.Value.KeyStorePath;
        var pattern = $"*.{GetFileExtension(keyUsage)}";

        var utcNow = _timeProvider.GetUtcNow();
        foreach (var fileName in Directory.EnumerateFiles(keyStorePath, pattern).ToList())
        {
            var key = await LoadKeyAsync(fileName, ct);
            if (key.Certificate.NotAfter > refDate)
                continue;

            File.Delete(fileName);
        }
    }



    private static string GetFileExtension(KeyUsage keyUsage) => keyUsage switch
    {
        KeyUsage.Signature => "sign.pfx",
        KeyUsage.Encryption => "enc.pfx",
        _ => throw new NotImplementedException()
    };
}
