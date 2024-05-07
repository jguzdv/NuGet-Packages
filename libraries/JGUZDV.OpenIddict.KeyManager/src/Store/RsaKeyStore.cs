using System.Security.Cryptography;
using System.Text.Json;

using JGUZDV.OpenIddict.KeyManager.Configuration;
using JGUZDV.OpenIddict.KeyManager.Model;

using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace JGUZDV.OpenIddict.KeyManager.Store
{
    internal sealed class RsaKeyStore : ProtectedFileStoreBase
    {
        private static readonly JsonSerializerOptions _jsonOptions = new() { IncludeFields = true };

        private readonly TimeProvider _timeProvider;

        protected override string FileExtension => "key";

        public RsaKeyStore(
            TimeProvider timeProvider,
            IDataProtectionProvider dataProtectionProvider, 
            IOptions<KeyManagerOptions> options, 
            ILogger<RsaKeyStore> logger) 
            : base(dataProtectionProvider, options, logger)
        {
            _timeProvider = timeProvider;
        }


        protected override sealed KeyInfo ConvertFromUnprotectedBytes(byte[] unprotectedBytes, string filepath, KeyUsage keyUsage)
        {
            var serializedKey = JsonSerializer.Deserialize<SerializedKey>(unprotectedBytes, _jsonOptions);

            return new KeyInfo(
                keyUsage, 
                new RsaSecurityKey(serializedKey.Parameters), 
                serializedKey.NotBefore, 
                serializedKey.NotBefore + _options.Value.MaxKeyAge);
        }


        protected override sealed byte[] ConvertToUnprotectedBytes(KeyInfo keyInfo)
        {
            if (keyInfo.SecurityKey is not RsaSecurityKey rsaSecurityKey)
            {
                throw new ArgumentException("SecurityKey must be of type RsaSecurityKey", nameof(keyInfo.SecurityKey));
            }

            var serializable  = new SerializedKey
            {
                Parameters = rsaSecurityKey.Parameters,
                NotBefore = keyInfo.NotBefore
            };

            return JsonSerializer.SerializeToUtf8Bytes(serializable, _jsonOptions);
        }

        protected override string GetKeyFileName(KeyInfo keyInfo) => $"{keyInfo.NotBefore:yyyyMMdd_HHmmss}";


        public class SerializedKey
        {
            public required RSAParameters Parameters { get; set; }
            public required DateTimeOffset NotBefore { get; set; }
        }
    }
}
