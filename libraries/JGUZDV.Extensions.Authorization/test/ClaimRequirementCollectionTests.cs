using System.Security.Claims;

namespace JGUZDV.Extensions.Authorization.Tests;

public class ClaimRequirementCollectionTests
{
    private readonly ClaimsPrincipal _testUser;
    private readonly ClaimRequirement[] _claimRequirements;

    public ClaimRequirementCollectionTests()
    {
        _testUser = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim("c1","v1"),
                new Claim("c2","v2")
            }, "TestAuthentication"
        ));

        _claimRequirements = new[]
        {
            new ClaimValueRequirement("c1","v1"),
            new ClaimValueRequirement("c2","v2"),
            new ClaimValueRequirement("c3","v3")
        };
    }

    [Fact]
    public void Any_Requirement_Collection_Succeeds()
    {
        var sut = new ClaimRequirementCollection(
            RequirementCollectionMatchType.MatchAny,
            _claimRequirements[2], _claimRequirements[0]
        );

        var result = sut.SatisfiesRequirement(_testUser);

        Assert.True(result);
    }

    [Fact]
    public void Any_Requirement_Collection_Fails()
    {
        var sut = new ClaimRequirementCollection(
            RequirementCollectionMatchType.MatchAny,
            _claimRequirements[2]
        );

        var result = sut.SatisfiesRequirement(_testUser);

        Assert.False(result);
    }

    [Fact]
    public void All_Requirement_Collection_Succeeds()
    {
        var sut = new ClaimRequirementCollection(
            RequirementCollectionMatchType.MatchAll,
            _claimRequirements[1], _claimRequirements[0]
        );

        var result = sut.SatisfiesRequirement(_testUser);

        Assert.True(result);
    }

    [Fact]
    public void All_Requirement_Collection_Fails()
    {
        var sut = new ClaimRequirementCollection(
            RequirementCollectionMatchType.MatchAll,
            _claimRequirements[2], _claimRequirements[0]
        );

        var result = sut.SatisfiesRequirement(_testUser);

        Assert.False(result);
    }

    [Fact]
    public void All_Empty_Requirement_Collection_Fails()
    {
        var sut = new ClaimRequirementCollection(
            RequirementCollectionMatchType.MatchAll
        );

        var result = sut.SatisfiesRequirement(_testUser);

        Assert.False(result);
    }
}
