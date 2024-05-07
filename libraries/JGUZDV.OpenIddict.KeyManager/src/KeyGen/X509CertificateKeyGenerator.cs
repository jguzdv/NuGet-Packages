using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

using JGUZDV.OpenIddict.KeyManager.Model;

using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace JGUZDV.OpenIddict.KeyManager.KeyGen;

internal class X509CertificateKeyGenerator : IKeyGenerator
{
    private readonly ILogger<X509CertificateKeyGenerator> _logger;

    public X509CertificateKeyGenerator(
        ILogger<X509CertificateKeyGenerator> logger)
    {
        _logger = logger;
    }

    public AsymmetricSecurityKey CreateKey(KeyUsage keyUsage, DateTimeOffset notBefore, DateTimeOffset notAfter)
    {
        using var algorithm = RSA.Create(keySizeInBits: 2048);

        var subject = keyUsage switch
        {
            KeyUsage.Signature => new X500DistinguishedName("CN=OIDC Signature Certificate"),
            KeyUsage.Encryption => new X500DistinguishedName("CN=OIDC Encryption Certificate"),
            _ => throw new NotImplementedException()
        };

        var request = new CertificateRequest(subject, algorithm, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

        request.CertificateExtensions.Add(keyUsage switch
        {
            KeyUsage.Signature => new X509KeyUsageExtension(X509KeyUsageFlags.DigitalSignature, critical: true),
            KeyUsage.Encryption => new X509KeyUsageExtension(X509KeyUsageFlags.KeyEncipherment, critical: true),
            _ => throw new NotImplementedException()
        });

        var certificate = request.CreateSelfSigned(notBefore, notAfter);
        _logger.LogInformation("Created new certificate with thumbprint {thumbprint}", certificate.Thumbprint);

        return new X509SecurityKey(certificate);
    }
}
