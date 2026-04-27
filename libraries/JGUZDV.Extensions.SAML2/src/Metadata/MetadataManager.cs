using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace JGUZDV.Extensions.SAML2.Metadata;


/// <summary>
/// This manager/hosted service is used to update the EntityDescriptor's for all
/// known/configured RelyingParties every hour. A table of timers (scheduled executors)
/// is used to repeat the fetch process for every known EntityId.
/// </summary>
public class MetadataManager<TSPMetadata> : IHostedService
{
    // EntityId -> Timer (scheduled executors)
    private readonly Dictionary<string, Timer> _timers = [];

    // Our metadata container where we need to replace the entries every hour.
    private readonly MetadataContainer<TSPMetadata> _metadataContainer;
    private readonly MetadataLoader<TSPMetadata> _metadataLoader;

    // The options contain the list of known relying parties to update.
    private readonly IOptionsMonitor<RelyingPartyOptions> _options;
    private readonly ILogger _logger;

    public MetadataManager(
        MetadataContainer<TSPMetadata> metadataContainer,
        MetadataLoader<TSPMetadata> metadataLoader,
        IOptionsMonitor<RelyingPartyOptions> options,
        ILogger<MetadataManager<TSPMetadata>> logger)
    {
        _metadataContainer = metadataContainer;
        _metadataLoader = metadataLoader;
        _options = options;
        _logger = logger;
    }


    /// <summary>
    /// Start timers (scheduled executors) to update metadata for all known relying parties regularly.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task StartAsync(CancellationToken cancellationToken)
    {
        foreach (var option in _options.Get(typeof(TSPMetadata).FullName).RelyingParties)
        {
            _timers[option.EntityId] = new Timer(
                async ctx => await ReloadMetadataEntry(option),
                null,
                TimeSpan.FromHours(1),
                TimeSpan.FromHours(1)
            );
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Reload and set/replace an EntityDescriptor in the MetadataContainer.
    /// Note: We do not reuse the asynchronous mechanic from the MetadataContainer built around
    /// GetByEntityId(...) and LoadMetadataAsync:
    /// 1) We want to catch read errors here, and do nothing further than log the error, but do not replace the current entry.
    /// 2) We must not run into the LoadMetadataAsync exception handling block that remove's our current EntityDescriptor entry.
    /// </summary>
    /// <param name="option"></param>
    /// <returns></returns>
    private async Task ReloadMetadataEntry(RelyingPartyEntry option)
    {
        try
        {
            var spMetadata = await _metadataLoader.LoadMetadataAsync(option);
            _ = _metadataContainer.AddOrReplace(option.EntityId, Task.FromResult(spMetadata));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unable to load and exchange metadata for entry {entityId}", option.EntityId);
        }
    }

    /// <summary>
    /// Disposes timers and 'ends' the metadata manager.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task StopAsync(CancellationToken cancellationToken)
    {
        foreach (var timer in _timers.Values)
        {
            timer.Dispose();
        }

        return Task.CompletedTask;
    }
}
