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


    internal List<X509SecurityKey> GetKeys(KeyUsage keyUsage)
        => LoadKeys(keyUsage);

    internal List<X509SecurityKey> GetCachedKeys(KeyUsage keyUsage) => _securityKeys[keyUsage];



    private List<X509SecurityKey> LoadKeys(KeyUsage keyUsage)
    {
        var keyStorePath = _options.Value.KeyStorePath;
        var pattern = $"*.{GetFileExtension(keyUsage)}";

        var utcNow = _timeProvider.GetUtcNow();
        var securityKeys = new List<X509SecurityKey>();
        foreach (var fileName in Directory.EnumerateFiles(keyStorePath, pattern))
        {
            var key = LoadKey(fileName);
            if (key.Certificate.NotAfter > utcNow)
                securityKeys.Add(key);
        }

        return securityKeys;
    }


    private static X509SecurityKey LoadKey(string fileName)
    {
        var certificateBytes = File.ReadAllBytes(fileName);
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
    }



    internal void PurgeExpiredKeys(KeyUsage keyUsage, DateTimeOffset refDate)
    {
        var keyStorePath = _options.Value.KeyStorePath;
        var pattern = $"*.{GetFileExtension(keyUsage)}";

        foreach (var fileName in Directory.EnumerateFiles(keyStorePath, pattern).ToList())
        {
            var key = LoadKey(fileName);
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
