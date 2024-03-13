using Azure.Monitor.OpenTelemetry.AspNetCore;
using Azure.Monitor.OpenTelemetry.Exporter;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for default configuration of AzureMonitor OpenTelemetry transfer.
/// </summary>
public static class JGUZDVAspNetCoreOpenTelemetryExtensions
{
    /// <summary>
    /// Adds default OpenTelemetry support for transitting data to Azure Monitor resources if a 
    /// config section "AzureMonitor" is available with the following configuration parameters:
    /// ConnectionString - Represents connection to a Azure Monitor resource to transfer telemetry.
    /// ServiceNamespace - A namespace string representing the running web application.
    /// ServiceName - A name string representing the running web application.
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException">If config parameters are not valid</exception>
    public static WebApplicationBuilder AddJGUZDVOpenTelemetry(this WebApplicationBuilder builder)
    {
        // Configure the OpenTelemetry tracer provider to add the resource attributes to all traces.
        // @see https://learn.microsoft.com/en-us/azure/azure-monitor/app/opentelemetry-configuration?tabs=aspnetcore
        var settings = builder.Configuration.GetSection("AzureMonitor");

        if (settings.Exists())
        {
            if (string.IsNullOrWhiteSpace(settings.GetValue<string>("ConnectionString")))
            {
                throw new ArgumentException("Found AzureMonitor settings node, but no ConnectionString setting is provided.");
            }

            var serviceNamespace = settings.GetValue<string>("ServiceNamespace");

            if (string.IsNullOrWhiteSpace(serviceNamespace))
            {
                serviceNamespace = builder.Environment.ApplicationName;
            }

            if (string.IsNullOrWhiteSpace(serviceNamespace))
            {
                throw new ArgumentException("Found AzureMonitor settings node, but no " +
                    "setting 'ServiceNamespace' is provided and no default application name was found.");
            }

            var serviceName = settings.GetValue<string>("ServiceName");

            if (string.IsNullOrWhiteSpace(serviceName))
            {
                serviceName = builder.Environment.ApplicationName;
            }

            if (string.IsNullOrWhiteSpace(serviceName))
            {
                throw new ArgumentException("Found AzureMonitor settings node, but no " +
                    "setting 'ServiceName' is provided and no default application name was found.");
            }

            // Attribute service.instance.id (CloudRoleInstance) defaults to host name/device name.
            var attributes = new Dictionary<string, object>() {
                { "service.namespace", settings.GetValue<string>("ServiceNamespace")! },
                { "service.name", settings.GetValue<string>("ServiceName")! }
            };

            // Add the OpenTelemetry telemetry service to the application.
            // This service will collect and send telemetry data to Azure Monitor.
            builder.Services.AddOpenTelemetry()
                .WithTracing(tracing =>
                {
                    tracing.AddAspNetCoreInstrumentation();
                    tracing.AddHttpClientInstrumentation();
                }).UseAzureMonitor(options =>
                {
                    options.ConnectionString = settings.GetValue<string>("ConnectionString")!;
                });

            builder.Services.ConfigureOpenTelemetryTracerProvider((sp, builder) =>
                builder.ConfigureResource(resourceBuilder =>
                    resourceBuilder.AddAttributes(attributes)));

            builder.Logging
                .AddOpenTelemetry(options =>
                {
                    options.SetResourceBuilder(ResourceBuilder.CreateEmpty().AddAttributes(attributes));
                    options.AddAzureMonitorLogExporter(opts =>
                    {
                        opts.ConnectionString = settings.GetValue<string>("ConnectionString")!;
                    });

                    options.IncludeFormattedMessage = true;
                    options.IncludeScopes = true;
                    options.ParseStateValues = true;
                });
        }

        return builder;
    }
}
