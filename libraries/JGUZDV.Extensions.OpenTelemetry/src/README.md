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

These required configuration settings should be configured in your appsettings.json using the follwing example:

~~~ JSON
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

**Explenation:** Since this Extension is primarily aimed at ETL processes, for which sending metrics is essential, the "OpenTelemetry" section as well as all the parameters are required.
ServiceNamespace should describe a summarizing border or boundary, and ServiceName should be the concrete and unique name of the application. 
The name and namespace parameters should be about 10 to 15 characters, the strings should not include any whitespace or special characters.

## Meter

Since configuring a meter is required, this section details how to implement the necessary services to report metrics.

**Steps:**
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
