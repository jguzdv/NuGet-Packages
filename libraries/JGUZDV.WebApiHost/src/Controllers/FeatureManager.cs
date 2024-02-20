using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement;

namespace JGUZDV.WebApiHost.Controllers
{
    /// <summary>
    /// Controller for feauture flags
    /// </summary>
    [ApiController]
    public class FeatureManager : ControllerBase
    {
        private readonly IFeatureManager _featureManager;

        /// <summary>
        /// .ctor
        /// </summary>
        public FeatureManager(IFeatureManager featureManager)
        {
            _featureManager = featureManager;
        }

        /// <summary>
        /// Returns a list of all features and their status for the current user.
        /// </summary>
        [HttpGet("app/features")]
        [Produces<FeatureList>]
        public async Task<IActionResult> GetFeatures()
        {
            var result = new List<Feature>();

            var featureNames = _featureManager.GetFeatureNamesAsync();
            await foreach(var featureName in featureNames)
            {
                var isEnabled = await _featureManager.IsEnabledAsync(featureName);
                result.Add(new Feature(featureName, isEnabled));
            }

            return Ok(new FeatureList(result));
        }

        /// <summary>
        /// Represents a feature and its status
        /// </summary>
        public record Feature(string Name, bool IsEnabled);

        /// <summary>
        /// Represents a list of features and their status
        /// </summary>
        public record FeatureList(IEnumerable<Feature> Features);
    }
}
