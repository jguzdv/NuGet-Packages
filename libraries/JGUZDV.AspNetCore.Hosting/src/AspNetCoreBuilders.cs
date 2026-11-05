using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;

using OpenTelemetry;

namespace JGUZDV.AspNetCore.Hosting;

/// <summary>
/// Wraps the various builders used in an ASP.NET Core application, providing access to the AuthenticationBuilder, AuthorizationBuilder, MvcBuilder, DataProtectionBuilder, and HealthChecksBuilder for configuring services.
/// </summary>
public class AspNetCoreBuilders
{
    /// <summary>
    /// Provides access to the AuthenticationBuilder for configuring authentication services.
    /// </summary>
    public AuthenticationBuilder? AuthenticationBuilder { get; internal set; }

    /// <summary>
    /// Provides access to the MvcBuilder for configuring MVC services.
    /// </summary>
    public IMvcBuilder? MvcBuilder { get; internal set; }

    /// <summary>
    /// Provides access to the DataProtectionBuilder for configuring data protection services.
    /// </summary>
    public IDataProtectionBuilder? DataProtectionBuilder { get; internal set; }

    /// <summary>
    /// Provides access to the HealthChecksBuilder for configuring health check services.
    /// </summary>
    public IHealthChecksBuilder? HealthChecksBuilder { get; internal set; }

    /// <summary>
    /// Provides access to the FeatureManagementBuilder for configuring feature management services.
    /// </summary>
    public IFeatureManagementBuilder? FeatureManagementBuilder { get; internal set; }

    /// <summary>
    /// Provides access to the OpenTelemetryBuilder for configuring OpenTelemetry services.
    /// </summary>
    public IOpenTelemetryBuilder? OpenTelemetryBuilder { get; internal set; }
}