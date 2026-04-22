using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace JGUZDV.Extensions.SAML2.SPMetadata;

/// <summary>
/// Encapsulates logic/data to handle EntityDescriptor's. An EntityDescriptor describes a
/// SAML IdP or Saml ServiceProvider/RelyingParty. @see https://en.wikipedia.org/wiki/SAML_metadata
/// The property _metadata is used to store EntityDescriptor's. The dictionary uses EntityId's as
/// keys, and takes a Task that determines an EntityDescriptor. We do not store the EntityDescriptor
/// directly, but store the Task that runs asynchronously, so if GetByEntityId(...) is called multiple
/// times, and fetching an EntityDescriptor is not yet completed, all threads wait for the same Task.
/// </summary>
public class MetadataContainer<TSPMetadata>
{
    // Stores EntityId -> Task. The Task tries to fetch an EntityDescriptor for the given EntityId.
    private readonly Dictionary<string, Task<TSPMetadata>> _metadata = [];
    private readonly MetadataLoader<TSPMetadata> _loader;

    // All relying parties we know, needed to validate the saml authn request.
    private readonly IOptionsMonitor<RelyingPartyOptions> _options;
    private readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    public MetadataContainer(
        MetadataLoader<TSPMetadata> loader,
        IOptionsMonitor<RelyingPartyOptions> options,
        ILogger<MetadataContainer<TSPMetadata>> logger)
    {
        _loader = loader;
        _options = options;
        _logger = logger;
    }

    /// <summary>
    /// Replaces the current metadata for the given entityId
    /// </summary>
    public Task<TSPMetadata> AddOrReplace(string entityId, Task<TSPMetadata> descriptor)
    {
        _metadata[entityId] = descriptor;
        return descriptor;
    }

    /// <summary>
    /// Creates a task to either fetch the currently stored EntityDescriptor, or otherwise to load it.
    /// This mechanic ensures that we try to load an EntityDescriptor before we continue processing
    /// the current authentication request.
    /// </summary>
    /// <param name="entityId">A given entityId (from a saml authentication request)</param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException">When the given entityId is unknown.</exception>
    public Task<TSPMetadata> GetByEntityId(string entityId)
    {
        var entry = _options.Get(typeof(TSPMetadata).FullName).RelyingParties.FirstOrDefault(entry => entry.EntityId == entityId)
            ?? throw new InvalidOperationException($"Unknown entityId {entityId}");

        if (!_metadata.TryGetValue(entityId, out var value))
        {
            value = AddOrReplace(entityId, LoadMetadataAsync(entry));
        }

        return value;
    }

    /// <summary>
    /// This runs asynchronously on the thread pool. Note: It is essential to remove the
    /// Tas&lt;TSPMetadata&gt; in case of an Exception, otherwise every call on GetByEntityId(...)
    /// will get the Task's exception result.
    /// </summary>
    /// <param name="entry"></param>
    /// <returns></returns>
    /// <exception cref="MetadataLoaderException"></exception>
    private async Task<TSPMetadata> LoadMetadataAsync(RelyingPartyEntry entry)
    {
        try
        {
            return await _loader.LoadMetadataAsync(entry);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected exception when loading metadata.");

            // This is essential, so on a new request a new Task will be created. If we miss this,
            // every call on GetByEntityId(...) will get the Task with this error/exception result.
            _metadata.Remove(entry.EntityId);

            throw new MetadataLoaderException("Unexpected exception when loading metadata.", ex);
        }
    }

}
