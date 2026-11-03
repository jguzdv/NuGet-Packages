using System.Diagnostics.Metrics;

using Microsoft.Extensions.Options;

namespace JGUZDV.AspNetCore.Extensions.OpenTelemetry
{
    /// <summary>
    /// Inherit from this instance for JguZdv telemetry to create counts/histograms/gauges.
    /// </summary>
    [Obsolete("This class is not to be used. Use DI to get an instance of Meter injected or create your own via IMeterFactory", true)]
    public abstract class AbstractJguZdvMeter
    {
        /// <summary>
        /// JguZdv OpenTelemetry options.
        /// </summary>
        protected readonly IOptions<AspNetCoreOpenTelemetryOptions> _options;

        /// <summary>
        /// The one and only meter instance that must be used to transmit values to Azure Monitor.
        /// </summary>
        protected Meter Meter { get; }


        /// <summary>
        /// Uses AspNetCoreOpenTelemetryOptions to create a default meter instance.
        /// </summary>
        /// <param name="options"></param>
        protected AbstractJguZdvMeter(IOptions<AspNetCoreOpenTelemetryOptions> options)
        {
            _options = options;
        }
    }
}
