using System.Security.Claims;

namespace JGUZDV.Extensions.Authorization
{
    public class NullRequirement : ClaimRequirement
    {
        public override NullRequirement Clone()
        {
            return new NullRequirement();
        }

        public override bool IsSatisfiedBy(ClaimsPrincipal? principal) 
            => false;
    }
}
