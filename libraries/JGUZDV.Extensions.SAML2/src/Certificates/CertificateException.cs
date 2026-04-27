namespace JGUZDV.Extensions.SAML2.Certificates;

/// <summary>
/// Exception to indicate that a certificate-related operation went wrong.
/// </summary>
public class CertificateException(string? message, Exception? innerException = null) 
    : Exception(message, innerException)
{ }
