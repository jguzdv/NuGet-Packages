namespace JGUZDV.OpenIddict.KeyManager.Model;

/// <summary>
/// The intended key usage
/// </summary>
public enum KeyUsage
{
    /// <summary>
    /// None or unknown - should not be used.
    /// </summary>
    None = 0,

    /// <summary>
    /// Intended for signature
    /// </summary>
    Signature,

    /// <summary>
    /// Intended for encryption
    /// </summary>
    Encryption
}
