using System.Security.Claims;
using System.Text.Encodings.Web;

using JGUZDV.AspNetCore.Hosting.Authorization;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace JGUZDV.AspNetCore.Hosting.Tests;

public class ClaimCollectionAuthorizationRequirementTest
{
    private ClaimsPrincipal CreateUser()
    {
        return new ClaimsPrincipal(new ClaimsIdentity([new Claim("role", "admin"),
            new Claim("role", "user"),
            new Claim("scope", "read"),
            new Claim("scope", "write"),
            new Claim("scope", "delete"),
            new Claim("scp", "delete"),
        ],
        "FakeAuth", "name", "role"));
    }

    [Theory,
        InlineData([new[] { "READ" }, true]),
        InlineData([new[] { "READ", "invalid" }, true]),
        InlineData([new[] { "invalid" }, false]),
    ]
    public async Task Test_ClaimCollectionAuthorizationRequirement_Any(string[] values, bool expectedResult)
    {
        var user = CreateUser();
        var requirement = new ClaimCollectionAuthorizationRequirement(
            ClaimCollectionAuthorizationRequirement.MatchType.Any,
            "scope",
            StringComparer.OrdinalIgnoreCase,
            values);

        var context = new AuthorizationHandlerContext(new[] { requirement }, CreateUser(), null);

        await requirement.HandleAsync(context);
        Assert.Equal(expectedResult, context.HasSucceeded);
    }

    [Theory,
        InlineData([new[] { "READ" }, true]),
        InlineData([new[] { "READ", "invalid" }, false]),
        InlineData([new[] { "invalid" }, false]),
    ]
    public async Task Test_ClaimCollectionAuthorizationRequirement_All(string[] values, bool expectedResult)
    {
        var user = CreateUser();
        var requirement = new ClaimCollectionAuthorizationRequirement(
            ClaimCollectionAuthorizationRequirement.MatchType.All,
            "scope",
            StringComparer.OrdinalIgnoreCase,
            values);
        var context = new AuthorizationHandlerContext(new[] { requirement }, CreateUser(), null);
        await requirement.HandleAsync(context);
        Assert.Equal(expectedResult, context.HasSucceeded);
    }

    [Fact]
    public async Task Authorization_Can_be_used_in_host()
    {
        var hostBuilder = JGUZDVHostApplicationBuilder.CreateWebApi([], ctx =>
        {
            ctx.Builder.WebHost.UseEnvironment("Test");
            ctx.Builder.WebHost.UseTestServer();
        });

        hostBuilder.Services.AddAuthentication("TestScheme")
            .AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>("TestScheme", null);
        hostBuilder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("TestPolicy", policy =>
            {
                policy.RequireAllScopes(new[] { "test-scope" }, "scp");
            });

            options.AddPolicy("TestPolicy-invalid", policy =>
            {
                policy.RequireAllScopes(new[] { "invalid" });
            });
        });

        var host = hostBuilder.Builder.Build();
        try
        {
            host.UseRouting();
            host.UseAuthentication();
            host.UseAuthorization();

            host.MapGet("/test-200", () => Results.Ok())
                .RequireAuthorization("TestPolicy");

            host.MapGet("/test-403", () => Results.Ok())
                .RequireAuthorization("TestPolicy-invalid");

            _ = host.RunAsync();

            var client = host.GetTestClient();
            var response200 = await client.GetAsync("/test-200");
            var response403 = await client.GetAsync("/test-403");

            Assert.Equal(StatusCodes.Status200OK, (int)response200.StatusCode);
            Assert.Equal(StatusCodes.Status403Forbidden, (int)response403.StatusCode);
        }
        catch
        {
            throw;
        }
        finally
        {
            host.Lifetime.StopApplication();
        }
    }

    private class TestAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public TestAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder)
            : base(options, logger, encoder)
        { }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            return Task.FromResult(
                AuthenticateResult.Success(
                    new AuthenticationTicket(
                        new ClaimsPrincipal(
                            new ClaimsIdentity(
                                [
                                    new Claim("user","user"),
                                    new Claim("scp","test-scope"),
                                ],
                                "TestScheme"
                            )
                        ), "TestScheme"
                    )
                )
            );
        }
    }
}
