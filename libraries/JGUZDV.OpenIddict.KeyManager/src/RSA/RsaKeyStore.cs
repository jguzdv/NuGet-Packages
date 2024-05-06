using System.Globalization;
using System.Security.Cryptography;
using System.Text;

using JGUZDV.OpenIddict.KeyManager.Configuration;

using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace JGUZDV.OpenIddict.KeyManager.RSA
{
    internal class RsaKeyStore : IKeyStore
    {
        private readonly IDataProtector _dataProtector;
        private readonly TimeProvider _timeProvider;
        private readonly IOptions<KeyManagerOptions> _options;
        private readonly ILogger<RsaKeyStore> _logger;

        private const string EncyptionFileExtension = "enc.key";
        private const string SignatureFileExtension = "sig.key";
        private const string FilePattern = "*.*.key";

        public RsaKeyStore(
            IDataProtectionProvider dataProtectionProvider,
            TimeProvider timeProvider,
            IOptions<KeyManagerOptions> options,
            ILogger<RsaKeyStore> logger
        )
        {
            _dataProtector = dataProtectionProvider.CreateProtector("KeyProtection");
            _timeProvider = timeProvider;
            _options = options;
            _logger = logger;
        }

        public async Task<List<KeyInfo>> LoadKeysAsync(CancellationToken ct = default)
        {
            var utcNow = _timeProvider.GetUtcNow();
            var utcNowString = utcNow.ToString("yyyyMMddHHmmss");
            var keyStorePath = _options.Value.KeyStorePath;

            var keyLoadTasks = Directory.EnumerateFiles(keyStorePath, FilePattern)
                .Select(x => LoadKeyAsync(x, ct))
                .ToList();

            await Task.WhenAll(keyLoadTasks);

            return keyLoadTasks.Where(x => x.IsCompletedSuccessfully)
                .Select(x => x.Result)
                .Where(x => x.NotAfter > utcNow)
                .ToList();
        }


        private async Task<KeyInfo> LoadKeyAsync(string filePath, CancellationToken ct)
        {
            var encryptedRsaBytes = await File.ReadAllBytesAsync(filePath, ct);
            var rsaBytes = _dataProtector.Unprotect(encryptedRsaBytes);

            var rsaJson = Encoding.UTF8.GetString(rsaBytes);
            var rsaParameters = System.Text.Json.JsonSerializer.Deserialize<RSAParameters>(rsaJson);

            var fileName = Path.GetFileNameWithoutExtension(filePath);
            var securityKey = new RsaSecurityKey(rsaParameters)
            {
                KeyId = fileName
            };

            var effectiveDate = DateTimeOffset.ParseExact(fileName, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);

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

            return new KeyInfo(keyUsage, securityKey, effectiveDate, effectiveDate + _options.Value.MaxKeyAge);
        }


        public async Task SaveKeyAsync(KeyInfo keyInfo, CancellationToken ct)
        {
            if(keyInfo.SecurityKey is not RsaSecurityKey rsaSecurityKey)
            {
                throw new ArgumentException("SecurityKey must be of type RsaSecurityKey", nameof(keyInfo.SecurityKey));
            }

            var fileName = Path.Combine(
                _options.Value.KeyStorePath,
                Path.ChangeExtension($"{keyInfo.NotBefore:yyyyMMddHHmmss}", GetFileExtension(keyInfo.KeyUsage))
            );

            var rsaParameters = rsaSecurityKey.Parameters;

            var jsonRsaParameters = System.Text.Json.JsonSerializer.Serialize(rsaParameters);
            var rsaBytes = Encoding.UTF8.GetBytes(jsonRsaParameters);

            var encryptedrsaBytes = _dataProtector.Protect(rsaBytes);

            await File.WriteAllBytesAsync(fileName, encryptedrsaBytes, ct);
            _logger.LogDebug("RsaSecurityKey ({keyUsage}) has been written as {fileName}", keyInfo.KeyUsage, fileName);
        }


        public async Task PurgeExpiredKeys(DateTimeOffset refDate, CancellationToken ct)
        {
            var keyStorePath = _options.Value.KeyStorePath;

            foreach (var fileName in Directory.EnumerateFiles(keyStorePath, FilePattern).ToList())
            {
                var key = await LoadKeyAsync(fileName, ct);
                if (key.NotAfter > refDate)
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
}
