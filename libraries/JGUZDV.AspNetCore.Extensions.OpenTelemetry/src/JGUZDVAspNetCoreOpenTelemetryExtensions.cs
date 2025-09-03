using Azure.Monitor.OpenTelemetry.AspNetCore;
using JGUZDV.AspNetCore.Extensions.OpenTelemetry;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using OpenTelemetry.Resources;
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
    /// <param name="builder"></param>
    /// <param name="logger"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException">If config parameters are not valid</exception>
    public static WebApplicationBuilder AddJGUZDVOpenTelemetry(this WebApplicationBuilder builder, ILogger logger)
    {
        // Get OpenTelemetry section from appsettings.
        var settings = builder.Configuration.GetSection("OpenTelemetry");

        // Its okay if there is no OpenTelemetry settings node, but we will display a warning because this may not be indended.
        if (!settings.Exists())
        {
            logger?.LogWarning("The JGUZDV OpenTelemetry library is included, but there are no settings to configure it. No telemetry will be sent.");
            return builder;
        }

        // As this is the most important option to get OT working, we will "fail fast" here with an appropriate message if it is missing.
        // @see also ValidateAndSetDefaults(...)
        if (string.IsNullOrWhiteSpace(settings.GetValue<string>("AzureMonitor:ConnectionString")))
        {
            throw new ArgumentException("Found OpenTelemetry settings section, but no AzureMonitor:ConnectionString setting is provided.");
        }

        // As the app is not built yet, we need to define our own options to use them here.
        var otOptions = settings.Get<AspNetCoreOpenTelemetryOptions>()
            ?? throw new ArgumentException("Cannot bind AspNetCoreOpenTelemetryOptions.");

        // Look for ServiceNamespace and ServiceName, as these properties are essential.
        ValidateAndSetDefaults(builder, otOptions);

        // Configure the OpenTelemetry tracer provider to add the resource attributes to all traces.
        // @see https://learn.microsoft.com/en-us/azure/azure-monitor/app/opentelemetry-configuration?tabs=aspnetcore
        // Note: Attribute service.instance.id (CloudRoleInstance) defaults to host name/device name.
        var attributes = new Dictionary<string, object>() {
            { "service.namespace", otOptions.ServiceNamespace },
            { "service.name", otOptions.ServiceName! }
        };

        // Add the OpenTelemetry telemetry service to the application.
        // This service will collect and send telemetry data to Azure Monitor.
        var otBuilder = builder.Services.AddOpenTelemetry();

        // Adds tracing, which (currently?) seems to include logging.
        otBuilder.WithTracing(tracing =>
        {
            tracing.AddAspNetCoreInstrumentation();
            tracing.AddHttpClientInstrumentation();

            logger?.LogInformation("JGUZDV OpenTelemetry: Added tracing with AspNetCore instrumentation and HttpClient instrumentation.");
        });


        if (otOptions.UseMeter != null)
        {
            otBuilder.WithMetrics(metrics =>
            {
                metrics.AddMeter(otOptions.UseMeter.MeterName);
            });

            logger?.LogInformation("JGUZDV OpenTelemetry: UseMeter active. Initialized a meter with " +
                $"name {otOptions.UseMeter.MeterName}");
        }
        else
        {
            logger?.LogInformation("JGUZDV OpenTelemetry: UseMeter not active. No metrics will be sent.");
        }

        // Configure the connection string.
        otBuilder.UseAzureMonitor(options =>
        {
            options.ConnectionString = otOptions.AzureMonitor.ConnectionString;

            if (otOptions.SamplingRatio is >= 0.0f and <= 1.0f)
            {
                options.SamplingRatio = otOptions.SamplingRatio.Value;
                logger?.LogInformation("JGUZDV OpenTelemetry: Set SamplingRatio value " +
                                       "to {SamplingRatio}.", otOptions.SamplingRatio.Value);
            }
        });

        // Configure the ResourceBuilder to add the custom resource attributes to all signals.
        // Custom resource attributes should be added AFTER AzureMonitor to override the default ResourceDetectors.
        otBuilder.ConfigureResource(resourceBuilder => resourceBuilder.AddAttributes(attributes));

        // Configure OpenTelemetry options for the "outside" world. Validation and defaults are set via postconfig.
        builder.Services.AddOptions<AspNetCoreOpenTelemetryOptions>()
            .BindConfiguration("OpenTelemetry")
            .PostConfigure(c => ValidateAndSetDefaults(builder, c));    // Defacto obsolete when we came here, but we reuse it for the defaults.
        
        logger?.LogInformation("JGUZDV OpenTelemetry: Configuration finished. Please check traces and values in Azure Portal.");
    
        return builder;
    }


    /// <summary>
    /// Validate and/or default the settings ConnectionString, ServiceNamespace, ServiceName and MeterName if metrics are used.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="options"></param>
    /// <exception cref="ArgumentException"></exception>
    private static void ValidateAndSetDefaults(WebApplicationBuilder builder, AspNetCoreOpenTelemetryOptions options)
    {
        if(string.IsNullOrWhiteSpace(options.AzureMonitor.ConnectionString))
        {
            throw new ArgumentException("OpenTelemetry is configured, but no ConnectionString was found.");
        }

        if (string.IsNullOrWhiteSpace(options.ServiceNamespace))
        {
            if (!string.IsNullOrWhiteSpace(builder.Environment.ApplicationName))
            {
                options.ServiceNamespace = builder.Environment.ApplicationName;
            }
            else
            {
                throw new ArgumentException("Found OpenTelemetry appsettings section, but no " +
                    "setting 'ServiceNamespace' is provided and no default application name was found in host environment.");
            }
        }

        if (string.IsNullOrWhiteSpace(options.ServiceName))
        {
            if (!string.IsNullOrWhiteSpace(builder.Environment.ApplicationName))
            {
                options.ServiceName = builder.Environment.ApplicationName;
            }
            else
            {
                throw new ArgumentException("Found OpenTelemetry appsettings section, but no " +
                    "setting 'ServiceName' is provided and no default application name was found in host environment.");
            }
        }        

        if(options.UseMeter != null && string.IsNullOrWhiteSpace(options.UseMeter.MeterName))
        {
            throw new ArgumentException("Found OpenTelemetry configuration for metrics, but MeterName is not provided!");
        }
    }
}
