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
    public void Requirement_can_be_satisfied()
    {
        var sut = new ClaimValueRequirement("sub", "subject");
        var result = sut.IsSatisfiedBy(_testUser);

        Assert.True(result);
    }
    
    [Fact]
    public void Requirement_can_fail()
    {
        var sut = new ClaimValueRequirement("sub", "false");
        var result = sut.IsSatisfiedBy(_testUser);

        Assert.False(result);
    }

    [Fact]
    public void Requirement_Type_Is_CI()
    { 
        var sut = new ClaimValueRequirement("SUB", "subject");
        var result = sut.IsSatisfiedBy(_testUser);

        Assert.True(result);
    }

    [Fact]
    public void Requirement_StringComparer_Can_Be_Changed()
    {
        var sut = new ClaimValueRequirement("SUB", "subject", false, StringComparison.InvariantCulture, StringComparison.Ordinal);
        var result = sut.IsSatisfiedBy(_testUser);

        Assert.False(result);

        var sut2 = new ClaimValueRequirement("sub", "suBjEcT", false,StringComparison.OrdinalIgnoreCase ,StringComparison.OrdinalIgnoreCase);
        var result2 = sut2.IsSatisfiedBy(_testUser);

        Assert.True(result2);

    }

    [Fact]
    public void Requirement_Value_Is_CS()
    {
        var sut = new ClaimValueRequirement("sub", "suBjEcT");
        var result = sut.IsSatisfiedBy(_testUser);

        Assert.False(result);
    }
    
    [Fact]
    public void Requirement_With_Wildcard_Type()
    {
        var sut = new ClaimValueRequirement("*", "subject");
        var result = sut.IsSatisfiedBy(_testUser);

        Assert.True(result);
    }
    
    [Fact]
    public void Requirement_With_Wildcard_Value()
    {
        var sut = new ClaimValueRequirement("sub", "*");
        var result = sut.IsSatisfiedBy(_testUser);

        Assert.True(result);
    }

    [Fact]
    public void Requirement_With_Wildcard_Type_But_Disabled_Wildcard()
    {
        var sut = new ClaimValueRequirement("*", "subject", disableWildcardMatch: true);
        var result = sut.IsSatisfiedBy(_testUser);

        Assert.False(result);
    }

    [Fact]
    public void Requirement_With_Wildcard_Value_But_Disabled_Wildcard()
    {
        var sut = new ClaimValueRequirement("sub", "*", disableWildcardMatch: true);
        var result = sut.IsSatisfiedBy(_testUser);

        Assert.False(result);
    }
}