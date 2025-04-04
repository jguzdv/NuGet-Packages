using System.Diagnostics.Metrics;

using Microsoft.Extensions.Options;

namespace JGUZDV.Extensions.OpenTelemetry;

/// <summary>
/// Inherit from this instance for JguZdv telemetry to create counts/histograms/gauges.
/// </summary>
public abstract class JGUZDVBaseMeter
{
    /// <summary>
    /// JguZdv OpenTelemetry options.
    /// </summary>
    protected readonly IOptions<JGUZDVOpenTelemetryOptions> _telemetryOptions;

    /// <summary>
    /// The one and only meter instance that must be used to transmit values to Azure Monitor.
    /// </summary>
    protected Meter Meter { get; }

    /// <summary>
    /// Use JGUZDVOpenTelemetryOptions to create a default meter instance.
    /// </summary>
    /// <param name="options"></param>
    protected JGUZDVBaseMeter(IOptions<JGUZDVOpenTelemetryOptions> options)
    {
        _telemetryOptions = options;

        Meter = new Meter(_telemetryOptions.Value.UseMeter!.MeterName, options.Value.UseMeter!.MeterVersion);
    }
}

