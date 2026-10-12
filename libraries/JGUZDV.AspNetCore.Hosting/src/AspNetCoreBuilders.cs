using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.Logging;

namespace JGUZDV.AspNetCore.Hosting;

public class AspNetCoreBuilders
{
    public AuthenticationBuilder? AuthenticationBuilder { get; set; }
    public AuthorizationBuilder? AuthorizationBuilder { get; set; }
    public IMvcBuilder? MvcBuilder { get; set; }
    public IDataProtectionBuilder? DataProtectionBuilder { get; set; }
    public IHealthChecksBuilder? HealthChecksBuilder { get; set; }
}