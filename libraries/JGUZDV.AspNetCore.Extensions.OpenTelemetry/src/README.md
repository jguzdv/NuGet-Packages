# JGUZDV.Extensions.AspNetCore.OpenTelemetry

Provides a simple preconfigured way for ASP.NET Core web applications to 
integrate OpenTelemetry transferring data to Azure Monitor resources.

To add OpenTelemetry support, just use the extension like this to add
the needed services to a WebApplicationBuilder:

´´´
builder.AddJGUZDVOpenTelemetry();
´´´

Basic configuration settings to initialize the services must be configured as 
in the following example:

´´´
  "OpenTelemetry": {
    "AzureMonitor": {
      "ConnectionString": "<AzureMonitorConnectionString>",
    },
    "ServiceNamespace": "<WebAppNamespace>",
    "ServiceName": "<WebAppName>"
  }
´´´

Note: If no "OpenTelemetry" section can be found, nothing will happen
at all. If such a section is present, the included parameters will be 
checked. ConnectionString is obviously a mandatory config parameter. 
WebAppNamespace should describe a summarizing border or boundary,
and WebAppName should be the concrete and unique name of the WebApp. 
The name and namespace parameters should be about 10 to 15 characters,
the strings should not include any whitespace or special characters.