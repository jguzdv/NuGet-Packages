using Microsoft.IdentityModel.Tokens;

namespace JGUZDV.OpenIddict.KeyManager.Model
{
    public class KeyContainer
    {
        private List<SecurityKey> _signatureKeys = new();
        private List<SecurityKey> _encryptionKeys = new();

        internal IReadOnlyList<SecurityKey> SignatureKeys => _signatureKeys;
        internal IReadOnlyList<SecurityKey> EncryptionKeys => _encryptionKeys;


        internal void ReplaceAllKeys(IEnumerable<KeyInfo> keyInfos)
        {
            _signatureKeys = new(
                keyInfos.Where(x => x.KeyUsage == KeyUsage.Signature)
                .Select(x => x.SecurityKey)
            );

            _encryptionKeys = new(
                keyInfos.Where(x => x.KeyUsage == KeyUsage.Encryption)
                .Select(x => x.SecurityKey)
            );
        }
    }
}
