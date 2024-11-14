# JGUZDV.Extensions.AspNetCore.OpenTelemetry

## General

Provides a simple preconfigured way for ASP.NET Core web applications to 
integrate OpenTelemetry transferring data to Azure Monitor resources.

## How to use

To add general OpenTelemetry support for your ASP.NET Core web app, just 
add the following line (builder is a WebApplicationBuilder):

~~~
builder.AddJGUZDVOpenTelemetry();
~~~

## How to configure

Basic configuration settings to initialize the services should be configured as 
in the following example in your appsettings (root level):

~~~
  ...
  "OpenTelemetry": {
    "AzureMonitor": {
      "ConnectionString": "<AzureMonitorConnectionString>"
    },
    "ServiceNamespace": "<WebAppNamespace>",
    "ServiceName": "<WebAppName>",
    "UseMeter": {
      "MeterName": "<WebAppMeterName>",
      "MeterVersion": "<WebAppMeterVersion>"
    }
  },
  ...
~~~

Explanation: If no "OpenTelemetry" section can be found, nothing will happen
at all. If such a section is present, the included parameters will be 
checked. ConnectionString is obviously a mandatory config parameter. 
WebAppNamespace should describe a summarizing border or boundary,
and WebAppName should be the concrete and unique name of the WebApp. 
The name and namespace parameters should be about 10 to 15 characters,
the strings should not include any whitespace or special characters. The
section "UseMeter" is optional, but if it is present, the two inner fields
are mandatory.

## Meter

Configure "UseMeter" to send metrics. To implement metrics, create a service
inheriting from AbstractJguZdvMeter, and add it to the service container as a
singleton (OpenTelemetry API's are thread-safe, according to the docs). 
This is what you should add to your Program.cs (complete example):

~~~
// Default OpenTelemetry config, needs the OpenTelemetry config section.
builder.AddJGUZDVOpenTelemetry();
builder.Services.AddSingleton<MeterContainer>();
~~~

Full example for a MeterContainer:

~~~
public class MeterContainer : AbstractJguZdvMeter
{
    private readonly Counter<int> _exampleCounter;


    public MeterContainer(IOptions<AspNetCoreOpenTelemetryOptions> options) : base(options)
    {
        _exampleCounter = Meter.CreateCounter<int>(
            name: "webappname.example.count",
            description: "An example counter for your webapp.");
    }


    public void CountExample()
    {
        _exampleCounter.Add(1)
    }
}
~~~

Note: 
* Please find meaningful names for your instruments!
* The needed options are prepared in the AddJGUZDVOpenTelemetry extension method. 
* The "Meter" instance is created in the base constructor.