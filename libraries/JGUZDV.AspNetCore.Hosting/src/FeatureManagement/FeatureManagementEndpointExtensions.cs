using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.FeatureManagement;

namespace JGUZDV.AspNetCore.Hosting.FeatureManagement;

/// <summary>
/// Extension methods for mapping FeatureManagement endpoints.
/// </summary>
public static class FeatureManagementEndpointExtensions
{
    /// <summary>
    /// Maps the FeatureManagement endpoints to the route.
    /// </summary>
    public static IEndpointRouteBuilder MapFeatureManagement(this IEndpointRouteBuilder route, string routePrefix = "_app/feature")
    {
        var featureApi = route.MapGroup(routePrefix);

        featureApi.MapGet("", 
            async (HttpContext context, FeatureManager _featureManager) =>
            {
                var result = new List<Feature>();

                var featureNames = _featureManager.GetFeatureNamesAsync();
                await foreach (var featureName in featureNames)
                {
                    var isEnabled = await _featureManager.IsEnabledAsync(featureName);
                    result.Add(new Feature(featureName, isEnabled));
                }

                return new FeatureList(result);
            });

        return route;
    }
}


