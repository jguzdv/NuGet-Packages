using IdentityModel.Client;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

using ZDV.BlazorWebTemplate.WebApi.HttpModel;

namespace ZDV.BlazorWebTemplate.Web
{
    public class CustomCookieAuthenticationEvents : CookieAuthenticationEvents
    {
        private readonly HttpClient _httpClient;

        public CustomCookieAuthenticationEvents(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public override async Task SigningIn(CookieSigningInContext context)
        {
            ArgumentNullException.ThrowIfNull(context.Principal);

            // Check if the access token is present
            var accessToken = context.Properties.GetTokenValue("access_token");
            ArgumentException.ThrowIfNullOrWhiteSpace(accessToken);

            // Call the backend to get additional user claims, that might be derived from configurations
            _httpClient.SetBearerToken(accessToken);

            var webApiUser = await _httpClient.GetFromJsonAsync<WebApiUser>("_app/user", context.HttpContext.RequestAborted);

            var newIdentity = new ClaimsIdentity(context.Principal.Identity);
            AddNewClaims(webApiUser, newIdentity);

            // Create a new ClaimsPrincipal with the new identity
            var newPrincipal = new ClaimsPrincipal(newIdentity);
            context.Principal = newPrincipal;
        }

        private void AddNewClaims(WebApiUser? webApiUser, ClaimsIdentity identity)
        {
            if(webApiUser == null)
            {
                return;
            }

            foreach(var apiClaim in webApiUser.Claims.Distinct())
            {
                if(!identity.HasClaim(x => x.Type == apiClaim.Type && x.Value == apiClaim.Value))
                {
                    identity.AddClaim(new Claim(apiClaim.Type, apiClaim.Value));
                }
            }
        }
    }
}
