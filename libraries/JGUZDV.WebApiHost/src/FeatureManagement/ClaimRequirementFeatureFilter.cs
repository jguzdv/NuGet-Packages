using JGUZDV.Extensions.Authorization;

using Microsoft.FeatureManagement;

namespace JGUZDV.WebApiHost.FeatureManagement
{
    [FilterAlias(nameof(ClaimRequirement))]
    internal class ClaimRequirementFeatureFilter : IFeatureFilter
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ClaimRequirementFeatureFilter(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Task<bool> EvaluateAsync(FeatureFilterEvaluationContext context)
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if(user == null)
            {
                return Task.FromResult(false);
            }

            var requirementOptions = new ClaimRequirementOptions();
            context.Parameters.Bind(requirementOptions);

            return Task.FromResult(user.SatisfiesRequirement(requirementOptions.ClaimRequirement));
        }
    }
}
