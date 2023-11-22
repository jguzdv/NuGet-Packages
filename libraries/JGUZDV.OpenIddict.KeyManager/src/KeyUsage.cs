using Microsoft.IdentityModel.Tokens;

namespace JGUZDV.OpenIddict.KeyManager;

public record KeyInfo(
    KeyUsage KeyUsage, 
    X509SecurityKey SecurityKey);

public enum KeyUsage
{
    None = 0,
    Signature,
    Encryption
}
