using System.Net.Http.Json;

using Microsoft.FeatureManagement;

namespace JGUZDV.AspNetCore.Hosting.FeatureManagement
{
    /// <summary>
    /// Uses an <see cref="HttpClient"/> to fetch feature definitions from a remote source, that is expected to return a <see cref="FeatureList"/>.
    /// </summary>
    public class RemoteFeatureDefinitionProvider : IFeatureDefinitionProvider
    {
        private readonly HttpClient _httpClient;
        private Task<FeatureList?>? _cachedFeatures;

        /// <summary>
        /// Initializes a new instance of <see cref="RemoteFeatureDefinitionProvider"/>.
        /// </summary>
        public RemoteFeatureDefinitionProvider(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <inheritdoc />
        async IAsyncEnumerable<FeatureDefinition> IFeatureDefinitionProvider.GetAllFeatureDefinitionsAsync()
        {
            var features = await LoadFeaturesAsync();
            if(features == null)
            {
                yield break;
            }

            foreach (var feature in features.Features)
            {
                yield return CreateFeatureDefinition(feature.Name, feature.IsEnabled);
            }
        }

        /// <inheritdoc />
        async Task<FeatureDefinition> IFeatureDefinitionProvider.GetFeatureDefinitionAsync(string featureName)
        {
            var features = await LoadFeaturesAsync();
            return features?.Features
                .Where(x => x.Name.Equals(featureName, StringComparison.OrdinalIgnoreCase))
                .Select(x => CreateFeatureDefinition(x.Name, x.IsEnabled))
                .FirstOrDefault() 
                ?? new FeatureDefinition() { Name = featureName };
        }

        private Task<FeatureList?> LoadFeaturesAsync()
        {
            if (_cachedFeatures != null)
            {
                return _cachedFeatures;
            }

            return _cachedFeatures = _httpClient.GetFromJsonAsync<FeatureList>("");
        }

        private static FeatureDefinition CreateFeatureDefinition(string featureName, bool isEnabled)
            => new()
            {
                Name = featureName,
                EnabledFor = isEnabled
                    ? new List<FeatureFilterConfiguration>() { new() { Name = "AlwaysOn" } }
                    : new List<FeatureFilterConfiguration>()
            };
    }
}
