using JGUZDV.OpenIddict.KeyManager.Configuration;
using JGUZDV.OpenIddict.KeyManager.Model;

using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace JGUZDV.OpenIddict.KeyManager.Store
{
    internal abstract class ProtectedFileStoreBase : FileStoreBase
    {
        private readonly IDataProtectionProvider _dataProtectionProvider;
        
        protected ProtectedFileStoreBase(
            IDataProtectionProvider dataProtectionProvider,
            IOptions<KeyManagerOptions> options,
            ILogger logger)
            : base(options, logger)
        {
            _dataProtectionProvider = dataProtectionProvider;
        }

        protected sealed override KeyInfo ConvertFromBytes(byte[] bytes, string filepath, KeyUsage keyUsage)
        {
            var protector = _dataProtectionProvider.CreateProtector("KeyProtection");
            var unprotectedBytes = protector.Unprotect(bytes);

            return ConvertFromUnprotectedBytes(unprotectedBytes, filepath, keyUsage);
        }

        protected abstract KeyInfo ConvertFromUnprotectedBytes(byte[] unprotectedBytes, string filepath, KeyUsage keyUsage);


        protected sealed override byte[] ConvertToBytes(KeyInfo keyInfo)
        {
            var unprotectedBytes = ConvertToUnprotectedBytes(keyInfo);

            var protector = _dataProtectionProvider.CreateProtector("KeyProtection");
            return protector.Protect(unprotectedBytes);
        }

        protected abstract byte[] ConvertToUnprotectedBytes(KeyInfo keyInfo);
    }
}
