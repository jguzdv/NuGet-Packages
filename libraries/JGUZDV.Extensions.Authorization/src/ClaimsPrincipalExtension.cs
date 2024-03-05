using System.Security.Claims;

namespace JGUZDV.Extensions.Authorization
{
    /// <summary>
    /// Extension methods for <see cref="ClaimsPrincipal"/>.
    /// </summary>
    public static class ClaimsPrincipalExtension
    {
        /// <summary>
        /// Determines if the current user satisfies the specified requirement.
        /// </summary>
        public static bool SatisfiesRequirement(this ClaimsPrincipal currentUser, ClaimRequirement requirement) 
            => requirement.IsSatisfiedBy(currentUser);
    }
}
