# JGUZDV.Extensions.OpenTelemetry

## General

Provides a simple preconfigured way for .NET applications to integrate OpenTelemetry transferring data to Azure Monitor resources.

## Usage

To add OpenTelemetry support to your .NET application, just add the following line while configuring your HostApplicationBuilder:

~~~ C#
builder.AddJGUZDVOpenTelemetry();
~~~

or

~~~ C#
Host.CreateApplicationBuilder()
    .AddJGUZDVOpenTelemetry();
~~~

## Configuration

Basic configuration settings to initialize the services should be configured as 
in the following example in your appsettings (root level):

~~~ JSON
  ...
  "OpenTelemetry": {
    "AzureMonitor": {
      "ConnectionString": "<AzureMonitorConnectionString>"
    },
    "ServiceNamespace": "<AppNamespace>",
    "ServiceName": "<AppName>",
    "Metrics": {
      "MeterName": "<AppMeterName>",
      "MeterVersion": "<AppMeterVersion>"
    }
  },
  ...
~~~

**Explanation:** If no "OpenTelemetry" section can be found, nothing will happen at all. 
If such a section is present, the included parameters will be checked.
ConnectionString is obviously a mandatory config parameter. 
WebAppNamespace should describe a summarizing border or boundary, and WebAppName should be the concrete and unique name of the WebApp. 
The name and namespace parameters should be about 10 to 15 characters, the strings should not include any whitespace or special characters.
The section "UseMeter" is optional, but if it is present, the two inner fields are mandatory.

## Meter

If you wish to send metrics for your application follow these steps:

- Configure "OpenTelemetry:UseMetrics" section in **appsettings.json**
- Implement a service inheriting from JGUZDVBaseMeter
- Add the service to the service container as singleton

The service registration should look like this:

~~~ C#
builder.AddJGUZDVOpenTelemetry();
builder.Services.AddSingleton<MeterContainer>();
~~~

MeterContainer should look something like this:

~~~ C#
public class MeterContainer : JGUZDVBaseMeter
{
    private readonly Counter<int> _exampleCounter;

    public MeterContainer(IOptions<JGUZDVOpenTelemetryOptions> options) : base(options)
    {
        _exampleCounter = Meter.CreateCounter<int>(
            name: "webappname.example.count",
            description: "An example counter for your webapp.");
    }

    // Call this to send count metrics.
    public void CountExample()
    {
        _exampleCounter.Add(1);
    }
}
~~~

Note: 
* Please find meaningful names for your instruments!
* The needed options are prepared in the AddJGUZDVOpenTelemetry extension method. 
* The "Meter" instance is created in the base constructor.
