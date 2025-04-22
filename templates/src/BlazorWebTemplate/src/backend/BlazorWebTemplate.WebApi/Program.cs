using System.Security.Claims;

using JGUZDV.AspNetCore.Hosting;
using JGUZDV.Extensions.Authorization;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Controllers;

using ZDV.BlazorWebTemplate.WebApi;
using ZDV.BlazorWebTemplate.WebApi.HttpModel;

var builder = JGUZDVHostApplicationBuilder.CreateWebApi(args);

var systemAdminOptions = new ClaimRequirementOptions();
builder.Configuration.Bind("SystemAdminRequirement", systemAdminOptions);

builder.Services.Configure<AuthorizationOptions>(options =>
{
    options.AddPolicy(AuthorizationPolicies.SystemAdmin, policy =>
    {
        policy.RequireAuthenticatedUser()
            .RequireAssertion(ctx => systemAdminOptions.ClaimRequirement.IsSatisfiedBy(ctx.User));
    });
});

var app = builder.BuildAndConfigureDefault();

app.MapGet(
    "_app/user",
    (ClaimsPrincipal user) =>
    {
        if(user.Identity?.IsAuthenticated != true)
        {
            return Results.Unauthorized();
        }

        var claims = user.Claims
            .Select(x => new WebApiClaim(x.Type, x.Value))
            .ToList();

        if(systemAdminOptions.ClaimRequirement.IsSatisfiedBy(user))
        {
            claims.Add(new WebApiClaim("SystemAdmin", "true"));
        }

        return TypedResults.Ok(new WebApiUser(claims));
    }
);

await app.RunAsync();

internal partial class Program { }
