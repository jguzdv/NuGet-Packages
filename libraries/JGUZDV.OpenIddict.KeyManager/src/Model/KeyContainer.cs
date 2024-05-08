using Microsoft.IdentityModel.Tokens;

namespace JGUZDV.OpenIddict.KeyManager.Model
{
    public class KeyContainer
    {
        private List<SecurityKey> _signatureKeys = new();
        private List<SecurityKey> _encryptionKeys = new();

        internal IReadOnlyList<SecurityKey> SignatureKeys => _signatureKeys;
        internal IReadOnlyList<SecurityKey> EncryptionKeys => _encryptionKeys;


        internal void ReplaceAllKeys(IEnumerable<KeyInfo> signatureKeys, IEnumerable<KeyInfo> encryptionKeys)
        {
            _signatureKeys = new(
                signatureKeys.Select(x => x.SecurityKey)
            );

            _encryptionKeys = new(
                encryptionKeys.Select(x => x.SecurityKey)
            );
        }
    }
}
