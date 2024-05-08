using System.Security;
using System.Security.Cryptography.X509Certificates;

using JGUZDV.OpenIddict.KeyManager.Configuration;
using JGUZDV.OpenIddict.KeyManager.Model;

using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace JGUZDV.OpenIddict.KeyManager.Store;

internal sealed class X509KeyStore : FileStoreBase
{
    private readonly IDataProtector _dataProtector;
    private readonly TimeProvider _timeProvider;
    private readonly IOptions<X509Options> _x509Options;

    public X509KeyStore(
        IDataProtectionProvider dataProtectionProvider,
        TimeProvider timeProvider,
        IOptions<KeyManagerOptions> options,
        IOptions<X509Options> x509Options,
        ILogger<X509KeyStore> logger)
        :base(options, logger)
    {
        _dataProtector = dataProtectionProvider.CreateProtector("KeyProtection");
        _timeProvider = timeProvider;
        _x509Options = x509Options;
    }

    protected override string FileExtension => "pfx";

    protected override KeyInfo ConvertFromBytes(byte[] bytes, string filepath, KeyUsage keyUsage)
    {
        var x509Cert = new X509Certificate2(bytes, _x509Options.Value.CertificatePassword);
        var securityKey = new X509SecurityKey(x509Cert);

        return new KeyInfo(keyUsage, securityKey, x509Cert.NotBefore, x509Cert.NotAfter);
    }

    protected override byte[] ConvertToBytes(KeyInfo keyInfo)
    {
        var securityKey = GetSecurityKey(keyInfo);
        return securityKey.Certificate.Export(X509ContentType.Pfx, _x509Options.Value.CertificatePassword);
    }

    protected override string GetKeyFileName(KeyInfo keyInfo)
    {
        var securityKey = GetSecurityKey(keyInfo);
        return $"{securityKey.Certificate.Thumbprint}";
    }

    private X509SecurityKey GetSecurityKey(KeyInfo keyInfo)
    {
        if(keyInfo.SecurityKey is not X509SecurityKey x509SecurityKey)
        {
            throw new ArgumentException("KeyInfo is not of type X509KeyInfo", nameof(keyInfo));
        }

        return x509SecurityKey;
    }

    private async Task<X509SecurityKey> LoadKeyAsync(string fileName, CancellationToken ct)
    {
        try
        {
            var encryptedCertificateBytes = await File.ReadAllBytesAsync(fileName, ct);
            var certificateBytes = _dataProtector.Unprotect(encryptedCertificateBytes);
            var certificate = new X509Certificate2(certificateBytes);

            return new X509SecurityKey(certificate);
        }
        catch (Exception ex)
        {
            if (ex is TaskCanceledException)
            {
                _logger.LogWarning("Loading of SecurityKey from {fileName} has been cancelled.", fileName);
            }
            else
            {
                _logger.LogError(ex, "Loading of SecurityKey from {fileName} has failed.", fileName);
            }

            throw;
        }
    }

}
