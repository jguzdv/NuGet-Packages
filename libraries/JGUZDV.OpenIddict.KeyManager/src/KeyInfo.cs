using Microsoft.IdentityModel.Tokens;

namespace JGUZDV.OpenIddict.KeyManager;

/// <summary>
/// Represents a key with its usage and security key.
/// </summary>
public class KeyInfo
{
    public KeyInfo(
        KeyUsage keyUsage,
        X509SecurityKey securityKey)
        :this(
             keyUsage, 
             securityKey, 
             securityKey.Certificate.NotBefore,
             securityKey.Certificate.NotAfter)
    { }


    public KeyInfo(
        KeyUsage keyUsage,
        AsymmetricSecurityKey securityKey,
        DateTimeOffset notBefore,
        DateTimeOffset notAfter
    )
    {
        KeyUsage = keyUsage;
        SecurityKey = securityKey;

        NotBefore = notBefore;
        NotAfter = notAfter;
    }

    public KeyUsage KeyUsage { get; }
    public AsymmetricSecurityKey SecurityKey { get; }

    public DateTimeOffset NotBefore { get; }
    public DateTimeOffset NotAfter { get; }
}