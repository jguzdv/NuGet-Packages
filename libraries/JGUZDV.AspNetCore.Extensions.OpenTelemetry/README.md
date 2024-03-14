# JGUZDV.Extensions.AspNetCore.OpenTelemetry

Provides a simple preconfigured way for ASP.NET Core web applications to 
integrate OpenTelemetry transferring data to Azure Monitor resources.

To add OpenTelemetry support, just use the extension like this to add
the needed services to a WebApplicationBuilder:

´´´
builder.AddJGUZDVOpenTelemetry();
´´´

Basic configuration settings needed to initialize the services are 
the following:

´´´
  "AzureMonitor": {
    "ConnectionString": "InstrumentationKey=<AzureMonitorInstrumentationKey>",
    "ServiceNamespace": "<WebAppNamespace>",
    "ServiceName": "<WebAppName>"
  }
´´´

Note: If no "AzureMonitoring" section can be found, nothing will happen
at all.