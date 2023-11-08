using Microsoft.IdentityModel.Tokens;

namespace JGUZDV.OpenIddict.KeyManager
{
    public class KeyContainer
    {
        private List<SecurityKey> _signatureKeys = new ();
        private List<SecurityKey> _encryptionKeys = new ();

        internal IReadOnlyList<SecurityKey> SignatureKeys => _signatureKeys;
        internal IReadOnlyList<SecurityKey> EncryptionKeys => _encryptionKeys;


        internal void ReplaceAllKeys(List<KeyInfo> keyInfos)
        {
            ReplaceKeyList(
                keyInfos.Where(x => x.KeyUsage == KeyUsage.Signature).Select(x => x.SecurityKey),
                ref _signatureKeys
                );

            ReplaceKeyList(
                keyInfos.Where(x => x.KeyUsage == KeyUsage.Encryption).Select(x => x.SecurityKey),
                ref _encryptionKeys
                );
        }


        private void ReplaceKeyList(IEnumerable<SecurityKey> keys, ref List<SecurityKey> keyList)
        {
            if(keys.Any())
            {
                var newKeys = new List<SecurityKey>(keyList);
                newKeys.AddRange(keys);
                keyList = newKeys;
            }
        }
    }
}
