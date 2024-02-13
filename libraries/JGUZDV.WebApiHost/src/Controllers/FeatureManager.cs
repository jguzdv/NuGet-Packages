using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement;

namespace JGUZDV.WebApiHost.Controllers
{
    [ApiController]
    public class FeatureManager : ControllerBase
    {
        private readonly IFeatureManager _featureManager;

        public FeatureManager(IFeatureManager featureManager)
        {
            _featureManager = featureManager;
        }

        [HttpGet("features")]
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

        public record Feature(string Name, bool IsEnabled);
        public record FeatureList(IEnumerable<Feature> Features);
    }
}
