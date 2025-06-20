﻿using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;

using JGUZDV.Extensions.Json.Converters;

namespace JGUZDV.Blazor.Components.Authentication
{
    /// <summary>
    /// Obsolete: Migrate to Components.Web and use PersistentAuthenticationStateProvider.
    /// </summary>
    [Obsolete("Migrate to Components.Web and use PersistentAuthenticationStateProvider")]
    public class DefaultFetchClaimsPrincipal : IFetchAuthenticationState
    {
        private readonly HttpClient _httpClient;
        private readonly string _uri;
        private readonly string _expiresAtClaimType;
        private readonly JsonSerializerOptions? _jsonSerializerOptions;

        /// <summary>
        /// Obsolete: Migrate to Components.Web and use PersistentAuthenticationStateProvider.
        /// </summary>
        public DefaultFetchClaimsPrincipal(HttpClient httpClient, 
            string uri = "_app/principal",
            string expiresAtClaimType = "exp",
            JsonSerializerOptions? jsonSerializerOptions = null)
        {
            _httpClient = httpClient;
            _uri = uri;
            _expiresAtClaimType = expiresAtClaimType;
            _jsonSerializerOptions = jsonSerializerOptions;
        }

        /// <summary>
        /// Obsolete: Migrate to Components.Web and use PersistentAuthenticationStateProvider.
        /// </summary>
        public async Task<(ClaimsPrincipal? User, DateTimeOffset? ExpiresAt)> FetchPrincipalAsync(CancellationToken ct)
        {
            var options = new JsonSerializerOptions(_jsonSerializerOptions ?? JsonSerializerOptions.Default);
            if (!options.Converters.Any(x => x is ClaimsPrincipalConverter))
                options.Converters.Add(new ClaimsPrincipalConverter());

            try
            {
                var principal = await _httpClient.GetFromJsonAsync<ClaimsPrincipal>(_uri, options, ct);
                if (principal == null)
                    return (null, null);

                var expiresAt = GetExpirationOrDefault(principal);
                return (principal, expiresAt);
            }
            catch
            {
                return (null, null);
            }
        }

        private DateTimeOffset? GetExpirationOrDefault(ClaimsPrincipal cp)
        {
            var expiresAtString = cp.FindFirst(_expiresAtClaimType)?.Value;
            return !string.IsNullOrWhiteSpace(expiresAtString) && long.TryParse(expiresAtString, out var expiresAt)
                ? DateTimeOffset.FromFileTime(expiresAt)
                : null;
        }
    }
}
