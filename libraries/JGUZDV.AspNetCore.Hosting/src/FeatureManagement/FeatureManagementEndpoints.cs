using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace JGUZDV.AspNetCore.Hosting.FeatureManagement;

public static class FeatureManagementEndpoints
{
    public static IEndpointRouteBuilder MapFeatureManagement(this IEndpointRouteBuilder route, string routePrefix = "_app/feature")
    {
        var featureApi = route.MapGroup(routePrefix);

        featureApi.MapGet("", async context =>
        {
            var result = new List<Feature>();

            var featureNames = _featureManager.GetFeatureNamesAsync();
            await foreach (var featureName in featureNames)
            {
                var isEnabled = await _featureManager.IsEnabledAsync(featureName);
                result.Add(new Feature(featureName, isEnabled));
            }

            return Ok(new FeatureList(result));
        });

        return route;
    }
}


