# JGUZDV.JobHost

**JGUZDV.JobHost** is a .NET package designed to simplify the creation of background jobs in a Windows service using the `IHostBuilder` and `Quartz.net`. You can easily register jobs with a cron schedule, and they will be executed in the background as part of a Windows service.

## Configuration
You can configure your schedules in the **appsettings.json**. The default sectionName is "HostedJobs"
```json
{
  "HostedJobs": {
    "MyJob": "* * * * * ?",
    "MyJob2": "* 0/5 * * * ?",
    "MyJob3": "false", //this prevents the job from being registered e.g. for appsettings.development.json
    "DisableDevelopmentJobSelection": "true" // optional: disables the job selestion screen and runs the jobs with the given schedule in appsettings.Development.json
  }
}
```

### OpenTelemetry configuration
This Package automatically adds OpenTelemetry to the background service using [JGUZDV.Extensions.OpenTelemetry](https://github.com/jguzdv/NuGet-Packages/tree/main/libraries/JGUZDV.Extensions.OpenTelemetry/src). Therefore a "OpenTelemetry" section is required within the **appsettings.json**.
Refer to **JGUZDV.Extensions.OpenTelemetry**'s README for further details on how to correctly configure and use OpenTelemetry.

## Usage
Your jobs must implement the **IJob** interface from **Quartz.net**.
To use JGUZDV.JobHost, you can create an IHostBuilder and register jobs with a cron schedule. 
The jobs will be executed in the background as part of a Windows service.

Here's a basic example:

```csharp
using Microsoft.Extensions.DependencyInjection;
using JGUZDV.JobHost;

class Program
{
    static void Main(string[] args)
    {
        var host = JobHost.CreateJobHostBuilder(args, configureWindowsService, quartzHostedServiceOptions)
            .AddHostedJob<MyJob>() // with the configuration above the job runs every second
            .AddHostedJob<MyJob2>() // with the configuration above the job runs every 5 minutes (at 0, 5, 10, 15.... etc Minutes)
            .Build();

        _ = host.RunAsync();
    }
}
```
