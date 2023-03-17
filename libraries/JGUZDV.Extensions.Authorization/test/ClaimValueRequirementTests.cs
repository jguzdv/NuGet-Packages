using System.Security.Claims;

namespace JGUZDV.Extensions.Authorization.Tests;

public class ClaimValueRequirementTests
{
    private readonly ClaimsPrincipal _testUser;

    public ClaimValueRequirementTests()
    {
        _testUser = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim("sub","subject")
            }, "TestAuthentication"
            )
        );
    }

    [Fact]
    public void Matching_Claim_Can_Satisfy_ClaimValueRequirement()
    {
        var sut = new ClaimValueRequirement("sub", "subject");
        var result = sut.SatisfiesRequirement(_testUser);

        Assert.True(result);
    }

    [Fact]
    public void TypeMatch_Is_CI()
    {
        var sut = new ClaimValueRequirement("SUB", "subject");
        var result = sut.SatisfiesRequirement(_testUser);

        Assert.True(result);
    }

    [Fact]
    public void ValueMatch_Is_CS()
    {
        var sut = new ClaimValueRequirement("sub", "suBjEcT");
        var result = sut.SatisfiesRequirement(_testUser);

        Assert.False(result);
    }

    [Fact]
    public void Wildcard_Type_Can_Satisfy_ClaimValueRequirement()
    {
        var sut = new ClaimValueRequirement("*", "subject");
        var result = sut.SatisfiesRequirement(_testUser);

        Assert.True(result);
    }
    
    [Fact]
    public void Wildcard_Value_Can_Satisfy_ClaimValueRequirement()
    {
        var sut = new ClaimValueRequirement("sub", "*");
        var result = sut.SatisfiesRequirement(_testUser);

        Assert.True(result);
    }

    [Fact]
    public void NonMatching_Claim_Cannot_Satisfy_ClaimValueRequirement()
    {
        var sut = new ClaimValueRequirement("sub", "will-not-match");
        var result = sut.SatisfiesRequirement(_testUser);

        Assert.False(result);
    }
}