using Azure.Monitor.OpenTelemetry.Exporter;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace JGUZDV.Extensions.OpenTelemetry;

/// <summary>
/// Extension methods for default configuration of OpenTelemetry/AzureMonitor transfer.
/// </summary>
public static class JGUZDVOpenTelemetryExtensions
{
    /// <summary>
    /// Adds default OpenTelemetry support for transitting data to Azure Monitor resources if a 
    /// config section "OpenTelemetry" is available with the following configuration parameters:
    /// AzureMonitor:ConnectionString - Represents connection to a Azure Monitor resource to transfer telemetry.
    /// ServiceNamespace - A namespace string representing the running web application.
    /// ServiceName - A name string representing the running web application.
    /// Metrics:MeterName - Name to use for the meter responsible for sending metrics such as counts/histograms/gauges regarding the application.
    /// Metrics:MeterVersion - Version of meter, change this when you change anything about what metrics you transmit.
    /// </summary>
    /// <param name="builder"></param>
    /// <returns>HostApplicationBuilder with fully configured OpenTelemetry if necessary sections are present in appsettings.</returns>
    /// <exception cref="ArgumentException">If config parameters are not valid</exception>
    public static HostApplicationBuilder AddJGUZDVOpenTelemetry(this HostApplicationBuilder builder)
    {
        // Create a logger to give some startup information. As OpenTelemetry and Azure Monitor are very 'quite',
        // these are the only information we will get during startup.
        using (var sp = builder.Services.BuildServiceProvider())
        {
            using var loggerFactory = sp.GetRequiredService<ILoggerFactory>();

            var logger = loggerFactory.CreateLogger("JGUZDV.Extensions.OpenTelemetry");

            // getting 'OpenTelemetry' section and from appsettings
            var telemetrySettings = builder.Configuration.GetSection("OpenTelemetry");

            // Its okay if there is no 'OpenTelemetry' settings node, but we will display a warning because this may not be indended.
            if (!telemetrySettings.Exists())
            {
                logger?.LogWarning("The JGUZDV OpenTelemetry library is included, but there are no settings to configure it. No telemetry will be sent.");
                return builder;
            }

            // 'ConnectionString' is absolutley required for further configuration of OpenTelemetry therefore we throw an exception if the 'OpenTelemetry' section is present without a ConnectionString.
            // @see also ValidateAndSetDefaults(...)
            if (string.IsNullOrWhiteSpace(telemetrySettings.GetValue<string>("AzureMonitor:ConnectionString")))
                throw new ArgumentException("Found 'OpenTelemetry' settings section, but no 'AzureMonitor:ConnectionString' setting is provided.");

            // defining options since app is not built yet
            var telemetryOptions = telemetrySettings.Get<JGUZDVOpenTelemetryOptions>()
                ?? throw new ArgumentException("Cannot bind JGUZDVOpenTelemetryOptions.");

            // either set ServiceNamespace and ServiceName from appsettings or default environment variables
            // and throw exception if neither is possible since these properties are essential for configuration
            ValidateAndSetDefaults(builder, telemetryOptions);

            // Configure the OpenTelemetry tracer provider to add the resource attributes to all traces.
            // @see https://learn.microsoft.com/en-us/azure/azure-monitor/app/opentelemetry-configuration?tabs=aspnetcore
            // Note: Attribute service.instance.id (CloudRoleInstance) defaults to host name/device name.
            var attributes = new Dictionary<string, object>() {
                { "service.namespace", telemetryOptions.ServiceNamespace },
                { "service.name", telemetryOptions.ServiceName! }
            };

            // Add the OpenTelemetry telemetry service to the application.
            // This service will collect and send telemetry data to Azure Monitor.
            var telemetryBuilder = builder.Services.AddOpenTelemetry();

            telemetryBuilder.WithTracing(tracing =>
            {
                tracing.AddHttpClientInstrumentation()
                    .AddAzureMonitorTraceExporter(options =>
                    {
                        options.ConnectionString = telemetryOptions.AzureMonitor.ConnectionString;
                    });
                logger?.LogInformation("JGUZDV OpenTelemetry: Added tracing with HttpClient instrumentation.");
            });

            // adding metrics support if 'UseMetrics' section is specified
            if (telemetryOptions.UseMeter != null)
            {
                telemetryBuilder.WithMetrics(metrics =>
                {
                    metrics.AddHttpClientInstrumentation()
                        .AddAzureMonitorMetricExporter(options =>
                        {
                            options.ConnectionString = telemetryOptions.AzureMonitor.ConnectionString;
                        })
                        .AddMeter(telemetryOptions.UseMeter!.MeterName);
                });

                logger?.LogInformation("JGUZDV OpenTelemetry: UseMeter active. Initialized a meter with " +
                    $"name {telemetryOptions.UseMeter!.MeterName}");
            }
            else
                logger?.LogInformation("JGUZDV OpenTelemetry: UseMeter not active. No metrics will be sent.");

            // Configure OpenTelemetry trace provider to use custom ResourceBuilder wich adds our own
            // attributes to all signals.
            builder.Services.ConfigureOpenTelemetryTracerProvider((sp, builder) =>
                builder.ConfigureResource(resourceBuilder =>
                    resourceBuilder.AddAttributes(attributes)));

            // Adding JGUZDVOpenTelemetry options to DI for when User inherits from JGUZDVBaseMeter.
            builder.Services.AddOptions<JGUZDVOpenTelemetryOptions>()
                .BindConfiguration("OpenTelemetry")
                .PostConfigure(c => ValidateAndSetDefaults(builder, c));

            // Adding OpenTelemetry to the applications LoggingBuilder
            builder.Logging.AddOpenTelemetry(options =>
            {
                options.SetResourceBuilder(ResourceBuilder.CreateEmpty().AddAttributes(attributes));
                options.AddAzureMonitorLogExporter(opts =>
                {
                    opts.ConnectionString = telemetryOptions.AzureMonitor.ConnectionString;
                });

                options.IncludeFormattedMessage = true;
                options.IncludeScopes = true;
                options.ParseStateValues = true;
            });

            logger?.LogInformation("JGUZDV OpenTelemetry: Added OpenTelemetry to applications LoggingBuilder.");
        }

        return builder;
    }

    /// <summary>
    /// Validate and/or default the settings ConnectionString, ServiceNamespace, ServiceName, MeterName and MeterVersion.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="telemetryOptions"></param>
    /// <exception cref="ArgumentException">If neither appsettings or environment contains any required value.</exception>
    private static void ValidateAndSetDefaults(HostApplicationBuilder builder, JGUZDVOpenTelemetryOptions telemetryOptions)
    {
        if (string.IsNullOrWhiteSpace(telemetryOptions.AzureMonitor.ConnectionString))
            throw new ArgumentException("'OpenTelemetry' is configured, but no 'ConnectionString' was found.");

        if (string.IsNullOrWhiteSpace(telemetryOptions.ServiceNamespace))
        {
            if (!string.IsNullOrWhiteSpace(builder.Environment.ApplicationName))
                telemetryOptions.ServiceNamespace = builder.Environment.ApplicationName;
            else
                throw new ArgumentException("Found 'OpenTelemetry' appsettings section, but no " +
                    "setting 'ServiceNamespace' is provided and no default application name was found in host environment.");
        }

        if (string.IsNullOrWhiteSpace(telemetryOptions.ServiceName))
        {
            if (!string.IsNullOrWhiteSpace(builder.Environment.ApplicationName))
                telemetryOptions.ServiceName = builder.Environment.ApplicationName;
            else
                throw new ArgumentException("Found OpenTelemetry appsettings section, but no " +
                    "setting 'ServiceName' is provided and no default application name was found in host environment.");
        }
    }
}

