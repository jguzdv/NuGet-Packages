using System.Security.Claims;

using JGUZDV.AspNetCore.Hosting.Authorization;

using Microsoft.AspNetCore.Authorization;

namespace JGUZDV.AspNetCore.Hosting.Tests
{
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
    }
}
