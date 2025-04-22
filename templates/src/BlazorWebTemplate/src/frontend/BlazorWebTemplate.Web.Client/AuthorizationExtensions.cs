using Microsoft.AspNetCore.Authorization;

namespace ZDV.BlazorWebTemplate.Web.Client;

public static class AuthorizationExtensions {
    public static class Policies {
        public const string Admin = "Admin";
    }

    public static void AddDefaultPolicies(this AuthorizationOptions options)
    {
        options.AddPolicy(Policies.Admin, policy =>
            policy
                .RequireAuthenticatedUser()
                .RequireRole("Admin")
        );
    }
}