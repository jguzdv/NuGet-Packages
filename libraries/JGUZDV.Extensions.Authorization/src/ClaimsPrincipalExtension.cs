using System.Security.Claims;

namespace JGUZDV.Extensions.Authorization
{
    public static class ClaimsPrincipalExtension
    {
        public static bool SatisfiesRequirement(this ClaimsPrincipal currentUser, ClaimRequirement requirement) 
            => requirement.IsSatisfiedBy(currentUser);
    }
}
