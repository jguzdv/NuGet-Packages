using System.Linq;

using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;

namespace JGUZDV.Blazor.Hosting.FeatureManagement
{
    /// <summary>
    /// Provides feature definitions from a <see cref="PersistentComponentState"/>.
    /// </summary>
    class PersistentStateFeatureDefinitionProvider : IFeatureDefinitionProvider
    {
        private readonly PersistentComponentState _applicationState;
        private readonly ILogger<PersistentStateFeatureDefinitionProvider> _logger;
        private static FeatureList? _cachedFeatures;

        /// <summary>
        /// Initializes a new instance of <see cref="PersistentStateFeatureDefinitionProvider"/>.
        /// </summary>
        public PersistentStateFeatureDefinitionProvider(
            PersistentComponentState applicationState, 
            ILogger<PersistentStateFeatureDefinitionProvider> logger)
        {
            _applicationState = applicationState;
            _logger = logger;
        }

        /// <inheritdoc />
        public IAsyncEnumerable<FeatureDefinition> GetAllFeatureDefinitionsAsync()
        {
            var features = GetFeaturesFromState();
            return features != null
                ? features.Features
                    .Select(x => CreateFeatureDefinition(x.Name, x.IsEnabled))
                    .ToAsyncEnumerable()
                : AsyncEnumerable.Empty<FeatureDefinition>();
        }

        /// <inheritdoc />
        public Task<FeatureDefinition> GetFeatureDefinitionAsync(string featureName)
        {
            var features = GetFeaturesFromState();
            return Task.FromResult(features?.Features
                .Where(x => x.Name.Equals(featureName, StringComparison.OrdinalIgnoreCase))
                .Select(x => CreateFeatureDefinition(x.Name, x.IsEnabled))
                .FirstOrDefault()
                ?? new FeatureDefinition() { Name = featureName }
            );
        }


        private FeatureDefinition CreateFeatureDefinition(string name, bool isEnabled)
            => new()
            {
                Name = name,
                EnabledFor = isEnabled
                    ? new List<FeatureFilterConfiguration>() { new() { Name = "AlwaysOn" } }
                    : new List<FeatureFilterConfiguration>()
            };


        private FeatureList? GetFeaturesFromState()
        {
            if(_cachedFeatures == null)
            {
                if (!_applicationState.TryTakeFromJson(nameof(FeatureList), out _cachedFeatures))
                {
                    _logger.LogWarning("Could not read feature list from persistent state.");
                }
            }

            return _cachedFeatures;
        }

        
    }
}
