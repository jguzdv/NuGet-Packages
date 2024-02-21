using System.Security.Claims;

namespace JGUZDV.Extensions.Authorization
{
    /// <summary>
    /// Represents a requirement that is never satisfied.
    /// </summary>
    public class NullRequirement : ClaimRequirement
    {
        /// <summary>
        /// Gets a singleton instance of the requirement.
        /// </summary>
        public static NullRequirement Instance { get; } = new NullRequirement();

        /// <inheritdoc />
        public override NullRequirement Clone()
            => new NullRequirement();

        /// <inheritdoc />
        public override bool Equals(ClaimRequirement? other)
            => other is NullRequirement;

        /// <summary>
        /// Will always return false for the null requirement.
        /// </summary>
        public override bool IsSatisfiedBy(IEnumerable<Claim>? principal) 
            => false;
    }
}
