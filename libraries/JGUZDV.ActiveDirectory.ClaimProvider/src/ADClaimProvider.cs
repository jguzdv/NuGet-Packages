using System.DirectoryServices;
using System.Runtime.Versioning;
using System.Security.Claims;
using System.Text.RegularExpressions;

using JGUZDV.ActiveDirectory.ClaimProvider.Configuration;
using JGUZDV.ActiveDirectory.ClaimProvider.PropertyConverters;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace JGUZDV.ActiveDirectory.ClaimProvider
{
    [SupportedOSPlatform("Windows")]
    public class ADClaimProvider
    {
        const string accountControlProperty = "userAccountControl";

        private readonly IPropertyConverterFactory _converterFactory;

        private readonly IOptions<ActiveDirectoryOptions> _adOptions;
        private readonly ILogger<ADClaimProvider> _logger;

        public ADClaimProvider(
            IPropertyConverterFactory converterFactory,
            IOptions<ActiveDirectoryOptions> adOptions,
            ILogger<ADClaimProvider> logger)
        {
            _converterFactory = converterFactory;
            _adOptions = adOptions;
            _logger = logger;
        }


        public List<(string Type, string Value)> GetClaims(ClaimsPrincipal subject)
        {
            var result = new List<(string Type, string Value)>();

            var propertyMaps = _adOptions.Value.ClaimMaps.ToList();
            var userDirectoryEntry = GetUserDirectoryEntry(subject, propertyMaps.Select(x => x.PropertyName));
            if (userDirectoryEntry == null)
                return result;
            
            foreach (var map in propertyMaps)
            {
                var claimValues = ConvertProperty(userDirectoryEntry, map.PropertyName);
                if (map.ClaimValueDenyList?.Any() == true)
                    claimValues = FilterValues(claimValues, denyList: map.ClaimValueDenyList);

                result.AddRange(claimValues.Select(x => (map.ClaimType, x)));
            }

            return result;
        }


        public bool IsUserActive(ClaimsPrincipal subject)
        {
            try
            {
                var userDirectoryEntry = GetUserDirectoryEntry(subject, new[] { accountControlProperty });
                if (userDirectoryEntry?.Properties[accountControlProperty][0] is int adsUserFlags)
                {
                    //See https://docs.microsoft.com/en-us/windows/win32/api/iads/ne-iads-ads_user_flag_enum ADS_UF_ACCOUNTDISABLE 
                    return (adsUserFlags & 0x2) != 2;
                }

                return false;
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Could not determine ActiveState of user.");
                return false;
            }
        }



        private static IEnumerable<object> GetPropertyValues(PropertyValueCollection propertyValues)
        {
            if (propertyValues.Value is not object[] values)
            {
                if (propertyValues.Value is null)
                    return Array.Empty<object>();

                values = new[] { propertyValues.Value };
            }

            return values;
        }


        private IEnumerable<string> ConvertProperty(DirectoryEntry userEntry, string propertyName)
        {
            var converter = _converterFactory.GetConverter(propertyName);
            var property = userEntry.Properties[propertyName];

            var result = converter.ConvertProperty(GetPropertyValues(property));
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


        // -- Helper methods --
        private string GetSubjectIdentifier(ClaimsPrincipal principal, string subjectClaimType)
        {
            var result = principal.FindFirst(subjectClaimType)?.Value;

            if (string.IsNullOrWhiteSpace(result))
                throw new InvalidOperationException($"Could not find claim of type '{subjectClaimType}' or 'sub'.");

            return result;
        }

        private DirectoryEntry? GetUserDirectoryEntry(ClaimsPrincipal principal, IEnumerable<string> propertiesToLoad)
        {
            var identifier = GetSubjectIdentifier(principal, _adOptions.Value.UserClaimType);
            var adConnection = _adOptions.Value.Connection;

            return ADHelper.FindUserDirectoryEntry(
                adConnection.Server, adConnection.BaseDN,
                _adOptions.Value.UserFilter.Replace("{0}", identifier),
                propertiesToLoad);
        }
    }
}
