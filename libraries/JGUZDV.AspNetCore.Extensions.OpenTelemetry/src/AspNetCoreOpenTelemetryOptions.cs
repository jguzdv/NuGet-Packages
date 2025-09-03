using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JGUZDV.AspNetCore.Extensions.OpenTelemetry
{
    /// <summary>
    /// JGUZDV Open Telemetry options.
    /// </summary>
    public class AspNetCoreOpenTelemetryOptions
    {
        /// <summary>
        /// Mandatory. Configuration for the transport to Azure Monitor.
        /// </summary>
        public OtAzureMonitorOptions AzureMonitor { get; set; } = default!;

        /// <summary>
        /// Mandatory. A namespace string representing the running application.
        /// </summary>
        public string ServiceNamespace { get; set; } = default!;

        /// <summary>
        /// Optional, but recommended. A unique name representing the running application. Falls back to builder.Environment.ApplicationName.
        /// </summary>
        public string? ServiceName { get; set; } = default!;

        /// <summary>
        /// Optional. Sets Azure Monitor sampling rate. Use values between 0.0f and 1.0f.
        /// </summary>
        public float? SamplingRatio { get; set; } = default!;
        
        /// <summary>
        /// Optional. Configures a basic env for metrics and creates a basic meter instance.
        /// </summary>
        public OtUseMeterOptions? UseMeter { get; set; } = default!;
    }

    /// <summary>
    /// Mandatory. Configuration for the transport to Azure Monitor.
    /// </summary>
    public class OtAzureMonitorOptions
    {
        /// <summary>
        /// Mandatory. The connection string of the Azure Monitor resource.
        /// </summary>
        public string ConnectionString { get; set; } = default!;
    }

    /// <summary>
    /// Optional UseMeter options to configure a Meter. A Meter is used to directly transfer
    /// telemetry - like statistics, volume data or quantity structures.
    /// You must not define UseMeter, but if it is defined, all inner properties are mandatory.
    /// </summary>
    public class OtUseMeterOptions
    {
        /// <summary>
        /// The name of the meter that will be added.
        /// </summary>
        public string MeterName { get; set; } = default!;

        /// <summary>
        /// The version of the meter that will be added.
        /// </summary>
        public string MeterVersion { get; set; } = default!;
    }
}
