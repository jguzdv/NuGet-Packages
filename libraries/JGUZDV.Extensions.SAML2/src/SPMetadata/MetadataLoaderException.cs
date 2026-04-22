
namespace JGUZDV.Extensions.SAML2.SPMetadata;

/// <summary>
/// Exception to indicate that metadata loading (fetching an EntityDescriptor) went wrong.
/// </summary>
public class MetadataLoaderException(string message, Exception? innerException = null) : Exception(message, innerException)
{
}
