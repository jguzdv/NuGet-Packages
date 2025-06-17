using Microsoft.AspNetCore.Authorization;

namespace JGUZDV.AspNetCore.Hosting.Authorization
{
    /// <summary>
    /// Authorization requirement that checks if the user has a specific claim with a specific value.
    /// The claim value can be a single value or a space separated list of values.
    /// </summary>
    public class ClaimCollectionAuthorizationRequirement : AuthorizationHandler<ClaimCollectionAuthorizationRequirement>, IAuthorizationRequirement
    {
        private MatchType Match { get; }
        private string ClaimType { get; }
        private StringComparer StringComparer { get; }
        private string[] AllowedValues { get; }

        /// <summary>
        /// The type of match to perform when checking the claim values.
        /// </summary>
        public enum MatchType
        {
            /// <summary>
            /// Match all allowed values - essentially AND.
            /// </summary>
            All,

            /// <summary>
            /// Match any of the allowed values - essentially OR.
            /// </summary>
            Any
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClaimCollectionAuthorizationRequirement"/> class.
        /// </summary>
        public ClaimCollectionAuthorizationRequirement(
            MatchType matchType,
            string claimType,
            StringComparer? stringComparer = null,
            params string[] values)
        {
            Match = matchType;
            ClaimType = claimType;
            StringComparer = stringComparer ?? StringComparer.Ordinal;
            if (values.Length == 0)
            {
                throw new ArgumentException("At least one value must be provided", nameof(values));
            }
            
            AllowedValues = values;

            
        }

        /// <inheritdoc />
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ClaimCollectionAuthorizationRequirement requirement)
        {
            var userClaimValues = context.User
                .FindAll(ClaimType)
                .SelectMany(c => c.Value.Split(' '));

            if (userClaimValues == null || !userClaimValues.Any())
            {
                return Task.CompletedTask;
            }

            if(Match == MatchType.All)
            {
                if (AllowedValues.All(v => userClaimValues.Contains(v, StringComparer)))
                {
                    context.Succeed(requirement);
                }
            }
            else if (Match == MatchType.Any)
            {
                if (AllowedValues.Any(v => userClaimValues.Contains(v, StringComparer)))
                {
                    context.Succeed(requirement);
                }
            }


            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{nameof(ClaimCollectionAuthorizationRequirement)}: match {Match} {ClaimType} values ({string.Join(",", AllowedValues)}).";
        }
    }
}
