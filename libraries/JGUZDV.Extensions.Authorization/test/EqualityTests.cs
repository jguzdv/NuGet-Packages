namespace JGUZDV.Extensions.Authorization.Tests
{
    public class EqualityTests
    {
        public class EqualityTestDescription
        {
            public EqualityTestDescription(bool shouldBeEqual)
            {
                ShouldBeEqual = shouldBeEqual;
            }

            public required ClaimRequirement CompareMe { get; init; }
            public required ClaimRequirement? CompareWith { get; init; }
            
            public bool ShouldBeEqual { get; }
        }

        public static TheoryData<EqualityTestDescription> TestData => new()
        {
            new EqualityTestDescription(false)
            {
                CompareMe = new NullRequirement(),
                CompareWith = new ClaimValueRequirement("t","v")
            },
            new EqualityTestDescription(false)
            {
                CompareMe = new NullRequirement(),
                CompareWith = new ClaimRequirementCollection(RequirementCollectionMatchType.MatchAny)
            },

            new EqualityTestDescription(true)
            {
                CompareMe = new ClaimValueRequirement("type","value"),
                CompareWith = new ClaimValueRequirement("type","value")
            },
            new EqualityTestDescription(true)
            {
                CompareMe = new ClaimRequirementCollection(RequirementCollectionMatchType.MatchAll, new ClaimValueRequirement("type","value"), new ClaimValueRequirement("type_1","value_!")),
                CompareWith = new ClaimRequirementCollection(RequirementCollectionMatchType.MatchAll, new ClaimValueRequirement("type","value"), new ClaimValueRequirement("type_1","value_!")),
            },

            new EqualityTestDescription(false)
            {
                CompareMe = new ClaimValueRequirement("type","value",true),
                CompareWith = new ClaimValueRequirement("type","value",false)
            },
            new EqualityTestDescription(false)
            {
                CompareMe = new ClaimRequirementCollection(RequirementCollectionMatchType.MatchAll, new ClaimValueRequirement("type","value"), new ClaimValueRequirement("type_1","value_!")),
                CompareWith = new ClaimRequirementCollection(RequirementCollectionMatchType.MatchAll, new ClaimValueRequirement("type_1","value_1"), new ClaimValueRequirement("type","value")),
            },


        };

        [Theory, MemberData(nameof(TestData))]
        public void ExamTests(EqualityTestDescription testDescription)
        {
            Assert.Equal(testDescription.ShouldBeEqual, testDescription.CompareMe.Equals(testDescription.CompareWith));
        }
    }
}
