using System.Security.Claims;

namespace JGUZDV.Extensions.Authorization
{
    public class NullRequirement : ClaimRequirement
    {
        public override bool IsSatisfiedBy(ClaimsPrincipal? principal) 
            => false;
    }
}
