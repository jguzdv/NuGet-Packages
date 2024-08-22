using Microsoft.IdentityModel.Tokens;

namespace JGUZDV.OpenIddict.KeyManager.Model
{
    public class KeyContainer
    {
        private List<KeyInfo> _signatureKeys = new();
        private List<KeyInfo> _encryptionKeys = new();

        internal IReadOnlyList<KeyInfo> SignatureKeys => _signatureKeys;
        internal IReadOnlyList<KeyInfo> EncryptionKeys => _encryptionKeys;


        internal void ReplaceAllKeys(
            IEnumerable<KeyInfo> signatureKeys, 
            IEnumerable<KeyInfo> encryptionKeys)
        {
            _signatureKeys = new(signatureKeys);
            _encryptionKeys = new(encryptionKeys);
        }
    }
}
