using Microsoft.IdentityModel.Tokens;

namespace JGUZDV.OpenIddict.KeyManager
{
    public interface IKeyGenerator
    {
        AsymmetricSecurityKey CreateKey(KeyUsage keyUsage, DateTimeOffset notBefore, DateTimeOffset notAfter);
    }
}