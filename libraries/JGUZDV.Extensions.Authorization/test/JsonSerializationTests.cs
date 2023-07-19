using System.Text.Json;

namespace JGUZDV.Extensions.Authorization.Tests;

public class JsonSerializationTests
{
    [Fact]
    public void ClaimRequirement_Will_Roundtrip()
    {
        ClaimRequirement claimRequirement = new ClaimRequirementCollection(RequirementCollectionMatchType.MatchAll,
            new ClaimValueRequirement("c1", "subject"),
            new ClaimValueRequirement("c2", "value")
            );

        var jsonString = JsonSerializer.Serialize(claimRequirement);
        var result = JsonSerializer.Deserialize<ClaimRequirement>(jsonString);

        Assert.IsType<ClaimRequirementCollection>(result);

        if(result is ClaimRequirementCollection crc)
        {
            Assert.Equal(2, crc.Requirements.Count);
            Assert.IsType<ClaimValueRequirement>(crc.Requirements[0]);
            Assert.IsType<ClaimValueRequirement>(crc.Requirements[1]);
        }
    }

    [Fact]
    public void NullClaimRequirement_Will_Roundtrip()
    {
        ClaimRequirement claimRequirement = new NullRequirement();

        var jsonString = JsonSerializer.Serialize(claimRequirement);
        var result = JsonSerializer.Deserialize<ClaimRequirement>(jsonString);

        Assert.IsType<NullRequirement>(result);
    }
}
