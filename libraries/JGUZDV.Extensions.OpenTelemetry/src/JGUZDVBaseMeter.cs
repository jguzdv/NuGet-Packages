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

        if (_telemetryOptions.Value.UseMeter == null)
            throw new ArgumentException("Can only be used if UseMeter is completly configured in appsettings.");

        if (string.IsNullOrWhiteSpace(_telemetryOptions.Value.UseMeter.MeterName)
                || string.IsNullOrWhiteSpace(_telemetryOptions.Value.UseMeter.MeterVersion))
            throw new ArgumentException("MeterName and MeterVersion must be set to reasonable values.");

        Meter = new Meter(_telemetryOptions.Value.UseMeter.MeterName, _telemetryOptions.Value.UseMeter.MeterVersion);
    }
}

