using System.Diagnostics.Metrics;

using Azure.Monitor.OpenTelemetry.Exporter;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using OpenTelemetry;
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
    /// The default configuration section name for OpenTelemetry settings in appsettings.json or other configuration sources.
    /// </summary>
    public const string DefaultOpenTelemetrySectionName = "OpenTelemetry";

    /// <summary>
    /// Adds default OpenTelemetry support for transitting data to Azure Monitor resources if a 
    /// config section "OpenTelemetry" is available with the following configuration parameters:
    /// AzureMonitor:ConnectionString - Represents connection to a Azure Monitor resource to transfer telemetry.
    /// ServiceNamespace - A namespace string representing the running web application.
    /// ServiceName - A name string representing the running web application.
    /// Metrics:MeterName - Name to use for the meter responsible for sending metrics such as counts/histograms/gauges regarding the application.
    /// Metrics:MeterVersion - Version of meter, change this when you change anything about what metrics you transmit.
    /// </summary>
    public static HostApplicationBuilder AddJGUZDVOpenTelemetry(this HostApplicationBuilder builder, string sectionName = DefaultOpenTelemetrySectionName)
    {
        // Create a logger to give some startup information. As OpenTelemetry and Azure Monitor are very 'quite',
        // these are the only information we will get during startup.
        using (var sp = builder.Services.BuildServiceProvider())
        {
            using var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger("JGUZDV.Extensions.OpenTelemetry");

            // Its okay if there is no 'OpenTelemetry' settings node, but we will display a warning because this may not be indended.
            if (!builder.Configuration.GetSection(sectionName).Exists())
            {
                logger.LogWarning("The JGUZDV OpenTelemetry library is included, but there are no settings to configure it. No telemetry will be sent.");
                return builder;
            }

            var otelOptions = builder.Services.AddAndGetOpenTelemetryOptions(
                builder.Environment, 
                sectionName);
            
            AddJGUZDVOpenTelemetry(builder.Services, otelOptions, builder.Logging, logger);
        }

        return builder;
    }

    /// <summary>
    /// Adds the OpenTelemetry options to the DI container and returns the options object.
    /// </summary>
    /// <returns>The options as "snapshot" of the current configuration.</returns>
    public static JGUZDVOpenTelemetryOptions AddAndGetOpenTelemetryOptions(
        this IServiceCollection services, 
        IHostEnvironment environment,
        string sectionName = "OpenTelemetry")
    {
        var optionsBuilder = services.AddOptions<JGUZDVOpenTelemetryOptions>()
                .BindConfiguration(sectionName)
                .PostConfigure(c => SetDefaults(environment, c))
                .ValidateDataAnnotations();

        using var provider = services.BuildServiceProvider();

        var options = provider
            .GetRequiredService<IOptions<JGUZDVOpenTelemetryOptions>>()
            .Value;

        return options;
    }

    /// <summary>
    /// Adds default OpenTelemetry support for transitting data to Azure Monitor resources if a 
    /// config section "OpenTelemetry" is available with the following configuration parameters:
    /// AzureMonitor:ConnectionString - Represents connection to a Azure Monitor resource to transfer telemetry.
    /// ServiceNamespace - A namespace string representing the running web application.
    /// ServiceName - A name string representing the running web application.
    /// Metrics:MeterName - Name to use for the meter responsible for sending metrics such as counts/histograms/gauges regarding the application.
    /// Metrics:MeterVersion - Version of meter, change this when you change anything about what metrics you transmit.
    /// </summary>
    /// <returns>IServiceCollection with fully configured OpenTelemetry if necessary sections are present in appsettings.</returns>
    /// <exception cref="ArgumentException">If config parameters are not valid</exception>
    public static IServiceCollection AddJGUZDVOpenTelemetry(
        this IServiceCollection services,
        JGUZDVOpenTelemetryOptions otelOptions,
        ILoggingBuilder loggingBuilder,
        ILogger logger,
        Action<JGUZDVOpenTelemetryOptions, OpenTelemetryBuilder>? configureBuilder = null)
    {
        if (!otelOptions.IsAzureMonitorConfigured)
        {
            logger.LogError("The JGUZDV OpenTelemetry library is included, but there is no 'AzureMonitor:ConnectionString' setting in the 'OpenTelemetry' section. No telemetry will be sent.");
        }

        // Configure the OpenTelemetry tracer provider to add the resource attributes to all traces.
        // @see https://learn.microsoft.com/en-us/azure/azure-monitor/app/opentelemetry-configuration?tabs=aspnetcore
        // Note: Attribute service.instance.id (CloudRoleInstance) defaults to host name/device name.
        var attributes = new Dictionary<string, object>() {
                { "service.namespace", otelOptions.ServiceNamespace! },
                { "service.name", otelOptions.ServiceName! }
            };

        // Add the OpenTelemetry telemetry service to the application.
        // This service will collect and send telemetry data to Azure Monitor.
        var telemetryBuilder = services.AddOpenTelemetry();

        telemetryBuilder.WithTracing(tracing =>
        {
            tracing
                .AddHttpClientInstrumentation()
                .AddSqlClientInstrumentation();

            if (otelOptions.IsAzureMonitorConfigured)
            {
                tracing.AddAzureMonitorTraceExporter(options =>
                {
                    options.ConnectionString = otelOptions.AzureMonitor.ConnectionString;
                });
            }
        });

        logger.LogInformation("JGUZDV OpenTelemetry: Added tracing with HttpClient and SqlClient instrumentation.");

        // adding metrics support if 'UseMetrics' section is specified
        if (otelOptions.UseMeter != null)
        {
            telemetryBuilder.WithMetrics(metrics =>
            {
                metrics
                    .AddRuntimeInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddSqlClientInstrumentation();

                if (otelOptions.IsAzureMonitorConfigured)
                {
                    metrics.AddAzureMonitorMetricExporter(options =>
                    {
                        options.ConnectionString = otelOptions.AzureMonitor.ConnectionString;
                    });
                }

                metrics.AddMeter(otelOptions.UseMeter!.MeterName);
            });

            //Add a singleton Meter instance to the DI container, which can be injected into other services.
            services.AddSingleton(sp => sp.GetRequiredService<IMeterFactory>().Create(otelOptions.UseMeter!.MeterName, otelOptions.UseMeter!.MeterVersion));

            logger.LogInformation("JGUZDV OpenTelemetry: UseMeter active. Initialized a meter with name {MeterName}", otelOptions.UseMeter!.MeterName);
        }
        else
        {
            logger.LogInformation("JGUZDV OpenTelemetry: UseMeter not active. No metrics will be sent.");
        }

        configureBuilder?.Invoke(otelOptions, telemetryBuilder);

        // Apply the custom resource attributes to all signals.
        telemetryBuilder.ConfigureResource(resourceBuilder =>
            resourceBuilder.AddAttributes(attributes));

        // Adding OpenTelemetry to the applications LoggingBuilder
        loggingBuilder.AddOpenTelemetry(options =>
        {
            options.SetResourceBuilder(ResourceBuilder.CreateEmpty().AddAttributes(attributes));

            if (otelOptions.IsAzureMonitorConfigured)
            {
                options.AddAzureMonitorLogExporter(opts =>
                {
                    opts.ConnectionString = otelOptions.AzureMonitor.ConnectionString;
                });
            }

            options.IncludeFormattedMessage = true;
            options.IncludeScopes = true;
            options.ParseStateValues = true;
        });

        logger.LogInformation("JGUZDV OpenTelemetry: Added OpenTelemetry to applications LoggingBuilder.");

        return services;
    }

    /// <summary>
    /// Validate and/or default the settings ConnectionString, ServiceNamespace, ServiceName, MeterName and MeterVersion.
    /// </summary>
    private static void SetDefaults(IHostEnvironment environment, JGUZDVOpenTelemetryOptions telemetryOptions)
    {
        if (string.IsNullOrWhiteSpace(telemetryOptions.ServiceNamespace))
        {
            telemetryOptions.ServiceNamespace = environment.ApplicationName;
        }

        if (string.IsNullOrWhiteSpace(telemetryOptions.ServiceName))
        {
            telemetryOptions.ServiceName = environment.ApplicationName;
        }
    }
}

