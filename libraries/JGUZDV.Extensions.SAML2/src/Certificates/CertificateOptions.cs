namespace JGUZDV.Extensions.SAML2.Certificates;

/// <summary>
/// Options for certifcate managers.
/// </summary>
public class CertificateOptions
{
    public required string CertificatesPath { get; set; }
    public string? CertificatePassword { get; set; }

    public TimeSpan CertificateRenewThreshold { get; set; } = TimeSpan.FromDays(15);
}
