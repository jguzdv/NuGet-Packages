using System.Security.Cryptography.X509Certificates;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace JGUZDV.OpenIddict.KeyManager;

public class X509KeyStore
{
    private const string EncyptionFileExtension = "enc.pfx";
    private const string SignatureFileExtension = "sig.pfx";
    private const string FilePattern = "*.*.pfx";

    private readonly TimeProvider _timeProvider;
    private readonly IOptions<KeyManagerOptions> _options;
    private readonly ILogger<X509KeyStore> _logger;

    public X509KeyStore(
        TimeProvider timeProvider,
        IOptions<KeyManagerOptions> options,
        ILogger<X509KeyStore> logger)
    {
        _timeProvider = timeProvider;
        _options = options;
        _logger = logger;
    }


    public async Task<List<KeyInfo>> LoadKeysAsync(CancellationToken ct = default)
    {
        var utcNow = _timeProvider.GetUtcNow();
        var keyStorePath = _options.Value.KeyStorePath;

        var keyLoadTasks = Directory.EnumerateFiles(keyStorePath, FilePattern)
            .Select(x => LoadKeyAsync(x, ct))
            .ToList();

        await Task.WhenAll(keyLoadTasks);

        return keyLoadTasks.Where(x => x.IsCompletedSuccessfully)
            .Select(x => x.Result)
            .Where(x => x.SecurityKey.Certificate.NotAfter > utcNow)
            .ToList();
    }


    private async Task<KeyInfo> LoadKeyAsync(string fileName, CancellationToken ct)
    {
        try
        {
            var certificateBytes = await File.ReadAllBytesAsync(fileName, ct);
            var certificate = new X509Certificate2(certificateBytes);

            KeyUsage keyUsage;
            if (fileName.EndsWith(SignatureFileExtension, StringComparison.OrdinalIgnoreCase))
            {
                keyUsage = KeyUsage.Signature;
            }
            else if (fileName.EndsWith(EncyptionFileExtension, StringComparison.OrdinalIgnoreCase))
            {
                keyUsage = KeyUsage.Encryption;
            }
            else
            {
                throw new InvalidOperationException($"Cannot identifiy KeyUsage of {fileName}");
            }

            return new KeyInfo(keyUsage, new X509SecurityKey(certificate));
        }
        catch (Exception ex)
        {
            if (ex is TaskCanceledException)
            {
                _logger.LogWarning("Loading of SecurityKey from {fileName} has been cancelled.", fileName);
            }
            else
            {
                _logger.LogError(ex, "Loading of SecurityKey from {fileName} has failed.", fileName);
            }

            throw;
        }
    }



    internal async Task SaveKeyAsync(KeyUsage keyUsage, X509SecurityKey securityKey, CancellationToken ct)
    {
        var fileName = Path.Combine(
            _options.Value.KeyStorePath,
            Path.ChangeExtension(securityKey.Certificate.Thumbprint, GetFileExtension(keyUsage))
        );

        var certificateBytes = securityKey.Certificate.Export(X509ContentType.Pkcs12, string.Empty);
        await File.WriteAllBytesAsync(fileName, certificateBytes, ct);

        _logger.LogDebug("X509SecurityKey ({keyUsage}) has been written as {fileName}", keyUsage, fileName);
    }



    internal async Task PurgeExpiredKeys(DateTimeOffset refDate, CancellationToken ct)
    {
        var keyStorePath = _options.Value.KeyStorePath;

        foreach (var fileName in Directory.EnumerateFiles(keyStorePath, FilePattern).ToList())
        {
            var key = await LoadKeyAsync(fileName, ct);
            if (key.SecurityKey.Certificate.NotAfter > refDate)
                continue;

            File.Delete(fileName);
        }
    }



    private static string GetFileExtension(KeyUsage keyUsage) => keyUsage switch
    {
        KeyUsage.Signature => SignatureFileExtension,
        KeyUsage.Encryption => EncyptionFileExtension,
        _ => throw new NotImplementedException()
    };
}
