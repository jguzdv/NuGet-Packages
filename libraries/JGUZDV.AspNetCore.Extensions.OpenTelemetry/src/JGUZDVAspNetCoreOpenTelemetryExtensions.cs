using Azure.Monitor.OpenTelemetry.AspNetCore;

using JGUZDV.Extensions.OpenTelemetry;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for default configuration of OpenTelemetry/AzureMonitor transfer.
/// </summary>
public static class JGUZDVAspNetCoreOpenTelemetryExtensions
{
    /// <summary>
    /// Adds default OpenTelemetry support for transmitting data to Azure Monitor resources if a 
    /// config section "OpenTelemetry" is available with the following configuration parameters:
    /// AzureMonitor:ConnectionString - Represents connection to a Azure Monitor resource to transfer telemetry.
    /// ServiceNamespace - A namespace string representing the running web application.
    /// ServiceName - A name string representing the running web application.
    /// </summary>
    /// <exception cref="ArgumentException">If config parameters are not valid</exception>
    public static WebApplicationBuilder AddJGUZDVOpenTelemetry(
        this WebApplicationBuilder builder, 
        ILogger logger, 
        string sectionName = JGUZDVOpenTelemetryExtensions.DefaultOpenTelemetrySectionName)
    {
        // Its okay if there is no 'OpenTelemetry' settings node, but we will display a warning because this may not be indended.
        if (!builder.Configuration.GetSection(sectionName).Exists())
        {
            logger.LogWarning("The JGUZDV OpenTelemetry library is included, but there are no settings to configure it. No telemetry will be sent.");
            return builder;
        }

        var otelOptions = builder.Services.AddAndGetOpenTelemetryOptions(
            builder.Environment,
            sectionName);

        builder.Services.AddJGUZDVOpenTelemetry(
            otelOptions,
            builder.Logging,
            logger,
            (options, telemetryBuilder) =>
            {
                telemetryBuilder.WithTracing(tracing =>
                {
                    tracing.AddAspNetCoreInstrumentation();
                });

                telemetryBuilder.WithMetrics(metrics =>
                {
                    metrics.AddAspNetCoreInstrumentation();
                });

                logger.LogInformation("JGUZDV OpenTelemetry: Added tracing with AspNetCore instrumentation.");

                if (otelOptions.IsAzureMonitorConfigured)
                {
                    // Configure the connection string.
                    telemetryBuilder.UseAzureMonitor(options =>
                    {
                        options.ConnectionString = otelOptions.AzureMonitor.ConnectionString;

                        var samplingRatio = builder.Configuration.GetSection(sectionName).GetValue<float?>("SamplingRatio");

                        if (samplingRatio is >= 0.0f and <= 1.0f)
                        {
                            options.SamplingRatio = samplingRatio.Value;
                            logger.LogInformation("JGUZDV OpenTelemetry: Set SamplingRatio value " +
                                                   "to {SamplingRatio}.", samplingRatio.Value);
                        }
                    });
                }
            }
        );

        logger.LogInformation("JGUZDV OpenTelemetry: Configuration finished. Please check traces and values in Azure Portal.");
    
        return builder;
    }
}
