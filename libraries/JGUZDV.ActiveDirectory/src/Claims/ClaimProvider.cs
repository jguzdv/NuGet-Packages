using System.DirectoryServices;
using System.Runtime.Versioning;
using System.Text.RegularExpressions;

using JGUZDV.ActiveDirectory.Configuration;

using Microsoft.Extensions.Options;

namespace JGUZDV.ActiveDirectory.Claims;

[SupportedOSPlatform("windows")]
internal class ClaimProvider : IClaimProvider
{
    private readonly IPropertyReader _propertyReader;
    private readonly IOptions<ClaimProviderOptions> _options;

    public ClaimProvider(
        IPropertyReader propertyReader,
        IOptions<ClaimProviderOptions> options)
    {
        _propertyReader = propertyReader;
        _options = options;
    }

    /// <summary>
    /// Get claims from Active Directory for the given subject.
    /// ClaimTypes will be filtered using the known claim sources.
    /// AD-Properties will be converted using the configured converters.
    /// </summary>
    public IEnumerable<(string Type, string Value)> GetClaims(DirectoryEntry directoryEntry, params string[] claimTypes)
    {
        var result = new List<(string Type, string Value)>();

        var propertyMaps = _options.Value.ClaimSources
            .Where(x => claimTypes.Contains(x.ClaimType, StringComparer.OrdinalIgnoreCase))
            .ToList();

        var properties = propertyMaps.Select(x => x.PropertyName).Distinct().ToArray();
        directoryEntry.RefreshCache(properties);

        foreach (var map in propertyMaps)
        {
            var claimValues = _propertyReader.ReadStrings(directoryEntry.Properties, map.PropertyName, map.OutputFormat);

            if (map.ClaimValueDenyList?.Any() == true)
            {
                claimValues = FilterValues(claimValues, denyList: map.ClaimValueDenyList);
            }

            result.AddRange(claimValues.Select(x => (map.ClaimType, x)));
        }

        return result;
    }


    private static IEnumerable<string> FilterValues(IEnumerable<string> claimValues, List<string> denyList)
    {
        var result = claimValues.AsEnumerable();
        foreach (var regexPattern in denyList)
        {
            var regex = new Regex(regexPattern, RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);

            result = result.Where(x => !regex.IsMatch(x)).ToList();
        }

        return result.ToArray();
    }


    public IEnumerable<string> GetProvidedClaimTypes(params string[] claimTypes)
        => _options.Value.ClaimSources.Select(x => x.ClaimType)
            .Intersect(claimTypes, StringComparer.OrdinalIgnoreCase)
            .ToList();
}
