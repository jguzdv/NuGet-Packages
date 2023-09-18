using System.Security.Claims;

namespace JGUZDV.Extensions.Authorization
{
    public class NullRequirement : ClaimRequirement
    {
        public static NullRequirement Instance { get; } = new NullRequirement();

        public override NullRequirement Clone()
            => new NullRequirement();

        public override bool Equals(ClaimRequirement? other)
            => other is NullRequirement;

        public override bool IsSatisfiedBy(ClaimsPrincipal? principal) 
            => false;
    }
}
