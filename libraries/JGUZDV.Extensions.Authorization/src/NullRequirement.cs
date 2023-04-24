using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace JGUZDV.Extensions.Authorization
{
    public class NullRequirement : ClaimRequirement
    {
        public override bool IsSatisfiedBy(ClaimsPrincipal principal)
        {
            return false;
        }
    }
}
