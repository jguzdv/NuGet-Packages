﻿namespace JGUZDV.Extensions.OpenTelemetry;

/// <summary>
/// JGUZDV Open Telemetry options.
/// </summary>
public class JGUZDVOpenTelemetryOptions
{
    /// <summary>
    /// Mandatory. Configuration for the transport to Azure Monitor.
    /// </summary>
    public OpenTelemetryAzureMonitorOptions AzureMonitor { get; set; } = default!;

    /// <summary>
    /// Optional. A namespace string representing the running application. Falls back to builder.Environment.ApplicationName.
    /// </summary>
    public string ServiceNamespace { get; set; } = default!;

    /// <summary>
    /// Optional, but recommended. A unique name representing the running application. Falls back to builder.Environment.ApplicationName.
    /// </summary>
    public string? ServiceName { get; set; } = default!;

    /// <summary>
    /// Optional. Configures a basic env for metrics and creates a basic meter instance.
    /// </summary>
    public OpenTelemetryUseMeterOptions? UseMeter { get; set; } = default!;
}

/// <summary>
/// Mandatory. Configuration for the transport to Azure Monitor.
/// </summary>
public class OpenTelemetryAzureMonitorOptions
{
    /// <summary>
    /// Mandatory. The connection string of the Azure Monitor resource.
    /// </summary>
    public string ConnectionString { get; set; } = default!;
}

/// <summary>
/// Optional. UseMeter options to configure a Meter. A Meter is used to directly transfer
/// telemetry - like statistics, volume data or quantity structures.
/// If defined, all inner properties are mandatory.
/// </summary>
public class OpenTelemetryUseMeterOptions
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
