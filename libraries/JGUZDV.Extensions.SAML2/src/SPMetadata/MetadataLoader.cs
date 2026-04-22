namespace JGUZDV.Extensions.SAML2.SPMetadata;

/// <summary>
/// Base class for metadata loaders. A metadata loader is responsible for fetching/parsing the EntityDescriptor for a given RelyingPartyEntry.
/// </summary>
/// <typeparam name="TSPMetadata"></typeparam>
public abstract class MetadataLoader<TSPMetadata>
{
    /// <summary>
    /// Loads metadata for the given RelyingPartyEntry. 
    /// The implementation of this method is responsible for fetching/parsing the EntityDescriptor for the given RelyingPartyEntry and returning an instance of TSPMetadata.
    /// </summary>
    public abstract Task<TSPMetadata> LoadMetadataAsync(RelyingPartyEntry entry);
}
