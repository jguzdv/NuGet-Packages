using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace JGUZDV.Extensions.Authorization
{
    public static class ClaimsPrincipalExtension
    {
        public static bool SatisfiesRequirement(this ClaimsPrincipal currentUser, ClaimRequirement requirement)
        {
            return requirement.IsSatisfiedBy(currentUser);
        }
    }
}
