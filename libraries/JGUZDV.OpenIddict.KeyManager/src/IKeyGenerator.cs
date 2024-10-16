using JGUZDV.OpenIddict.KeyManager.Model;

using Microsoft.IdentityModel.Tokens;

namespace JGUZDV.OpenIddict.KeyManager
{
    /// <summary>
    /// Interface for generating keys.
    /// </summary>
    public interface IKeyGenerator
    {
        /// <summary>
        /// Creates a new key, if possible keyUsage, notBefore and notAfter are embedded into the key.
        /// </summary>
        AsymmetricSecurityKey CreateKey(KeyUsage keyUsage, DateTimeOffset notBefore, DateTimeOffset notAfter);
    }
}