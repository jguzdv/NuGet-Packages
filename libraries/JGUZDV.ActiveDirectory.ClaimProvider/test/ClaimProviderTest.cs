using System.Security.Claims;

using JGUZDV.ActiveDirectory.ClaimProvider.Configuration;
using JGUZDV.ActiveDirectory.ClaimProvider.PropertyConverters;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

using Xunit;

using static JGUZDV.ActiveDirectory.ClaimProvider.Tests.ClaimProviderTest;

namespace JGUZDV.ActiveDirectory.ClaimProvider.Tests;

public class ClaimProviderTest : IClassFixture<ClaimProviderFixture>
{
    public class ClaimProviderFixture
    {
        public ClaimProviderFixture()
        {
            var s = new ServiceCollection();
            s.AddConverters();

            Principal = new ClaimsPrincipal(new ClaimsIdentity(
                new[] { new Claim("name", "glatzert") },
                "test",
                "name",
                "role"));

            var options = new ActiveDirectoryOptions();

            options.Connection = new()
            {
                BaseDN = "OU=User,DC=Uni-Mainz,DC=DE"
            };

            options.UserFilter = "(samAccountName={0})";
            options.UserClaimType = "name";

            options.ClaimSources.Clear();
            options.ClaimSources.AddRange(
                options.Properties.Select(x =>
                    new ClaimSource(x.PropertyName, x.PropertyName))
                );


            Options = new OptionsWrapper<ActiveDirectoryOptions>(options);

            ConverterFactory = new PropertyConverterFactory(
                Options,
                NullLogger<PropertyConverterFactory>.Instance, 
                s.BuildServiceProvider().GetServices<IPropertyConverter>());
        }

        internal OptionsWrapper<ActiveDirectoryOptions> Options { get; }
        internal PropertyConverterFactory ConverterFactory { get; }
        public ClaimsPrincipal Principal { get; }
    }

    public class ADPropertyData : TheoryData<ADPropertyInfo>
    {
        public ADPropertyData()
        {
            foreach (var propertyConverter in Defaults.KnownProperties)
                this.Add(propertyConverter);
        }
    }

    public static TheoryData<ADPropertyInfo> TestData = new ADPropertyData();

    private readonly ClaimProviderFixture _fixture;

    public ClaimProviderTest(ClaimProviderFixture fixture)
    {
        _fixture = fixture;
    }

    [Theory, MemberData(nameof(TestData))]
    public void PropertyWillBeLoadedAndConverted(ADPropertyInfo data)
    {
        var adClaimProvider = new ADClaimProvider(_fixture.ConverterFactory, _fixture.Options, NullLogger<ADClaimProvider>.Instance);
        var result = adClaimProvider.GetClaims(_fixture.Principal, data.PropertyName);

        Assert.NotEmpty(result);
        Assert.Equal(data.PropertyName, result[0].Type);
        Assert.NotEmpty(result[0].Value);
    }
}
