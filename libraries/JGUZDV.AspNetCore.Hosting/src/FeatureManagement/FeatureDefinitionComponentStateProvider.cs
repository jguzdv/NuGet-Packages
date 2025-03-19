using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using JGUZDV.AspNetCore.Hosting.Components;

using Microsoft.AspNetCore.Components;
using Microsoft.FeatureManagement;

namespace JGUZDV.AspNetCore.Hosting.FeatureManagement
{
    /// <summary>
    /// Saves a <see cref="FeatureList"/> to the persistent component state.
    /// </summary>
    public class FeatureDefinitionComponentStateProvider : IPersistentComponentStateProvider
    {
        private readonly IFeatureManager _featureManager;

        /// <summary>
        /// Creates a new instance of the <see cref="FeatureDefinitionComponentStateProvider"/>.
        /// </summary>
        /// <param name="featureManager"></param>
        public FeatureDefinitionComponentStateProvider(IFeatureManager featureManager)
        {
            _featureManager = featureManager;
        }

        /// <inheritdoc />
        public async Task PersistStateAsync(PersistentComponentState applicationState)
        {
            var features = new List<Feature>();
            await foreach(var feature in _featureManager.GetFeatureNamesAsync())
            {
                var isEnabled = await _featureManager.IsEnabledAsync(feature);

                features.Add(new Feature(feature, isEnabled));
            }

            applicationState.PersistAsJson(nameof(FeatureList), new FeatureList(features));
        }
    }
}
