﻿using System.DirectoryServices;
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


        /// <summary>
        /// Get claims from Active Directory for the given subject.
        /// ClaimTypes will be filtered using the known claim sources.
        /// AD-Properties will be converted using the configured converters.
        /// </summary>
        public List<(string Type, string Value)> GetClaims(ClaimsPrincipal subject, params string[] claimTypes)
        {
            var result = new List<(string Type, string Value)>();

            var propertyMaps = _adOptions.Value.ClaimSources
                .Where(x => claimTypes.Contains(x.ClaimType, StringComparer.OrdinalIgnoreCase))
                .ToList();

            var userDirectoryEntry = GetUserDirectoryEntry(subject, propertyMaps.Select(x => x.PropertyName));
            if (userDirectoryEntry == null)
                return result;
            
            foreach (var map in propertyMaps)
            {
                var claimValues = ConvertProperty(userDirectoryEntry, map);
                if (map.ClaimValueDenyList?.Any() == true)
                    claimValues = FilterValues(claimValues, denyList: map.ClaimValueDenyList);

                result.AddRange(claimValues.Select(x => (map.ClaimType, x)));
            }

            return result;
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


        private IEnumerable<string> ConvertProperty(DirectoryEntry userEntry, ClaimSource claimSource)
        {
            var converter = _converterFactory.GetConverter(claimSource.PropertyName, claimSource.OutputFormat);
            var property = userEntry.Properties[claimSource.PropertyName];

            var result = converter.ConvertProperty(GetPropertyValues(property), claimSource.OutputFormat);
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

            var (isBindable, bindPath) = ADHelper.IsBindableIdentity(identifier);
            if (isBindable)
            {
                return ADHelper.BindDirectoryEntry(adConnection.Server, bindPath, propertiesToLoad);
            }

            if (string.IsNullOrWhiteSpace(_adOptions.Value.UserFilter))
                throw new InvalidOperationException("UserFilter is not configured.");

            return ADHelper.FindUserDirectoryEntry(
                adConnection.Server, 
                adConnection.BaseDN,
                _adOptions.Value.UserFilter.Replace("{0}", identifier),
                propertiesToLoad);
        }
    }
}
