using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Options;

namespace JGUZDV.AspNetCore.Extensions.OpenTelemetry
{
    /// <summary>
    /// Inherit from this instance for JguZdv telemetry to create counts/histograms/gauges.
    /// </summary>
    public abstract class AbstractJguZdvMeter
    {
        /// <summary>
        /// JguZdv OpenTelemetry options.
        /// </summary>
        protected readonly IOptions<AspNetCoreOpenTelemetryOptions> _options;

        /// <summary>
        /// The one and only meter instance that must be used to transmit values to Azure Monitor.
        /// </summary>
        protected readonly Meter _otMeter;


        /// <summary>
        /// Uses AspNetCoreOpenTelemetryOptions to create a default meter instance.
        /// </summary>
        /// <param name="options"></param>
        protected AbstractJguZdvMeter(IOptions<AspNetCoreOpenTelemetryOptions> options)
        {
            _options = options;

            if(_options.Value.UseMeter == null)
            {
                throw new ArgumentException("Can only be used if UseMeter is completly configured in appsettings.");
            }

            _otMeter = new Meter(_options.Value.UseMeter!.MeterName, options.Value.UseMeter!.MeterVersion);
        }
    }
}
