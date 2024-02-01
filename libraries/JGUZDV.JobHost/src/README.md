# JGUZDV.JobHost

[![NuGet Version](https://img.shields.io/nuget/v/JGUZDV.JobHost.svg)](https://www.nuget.org/packages/JGUZDV.JobHost/)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

## Overview

**JGUZDV.JobHost** is a .NET package designed to simplify the creation of background jobs in a Windows service using the `IHostBuilder` and `Quartz.net`. You can easily register jobs with a cron schedule, and they will be executed in the background as part of a Windows service.

## Installation

You can install the package via NuGet Package Manager Console by running the following command:

```bash
Install-Package JGUZDV.JobHost
Or using the .NET CLI:

dotnet add package JGUZDV.JobHost
```

## Configuration
You can configure your schedules in the **appsettings.json**. The default sectionName is "HostedJobs"
```json
{
  "HostedJobs": {
    "MyJob": "* * * * * ?",
    "MyJob2": "* 0/5 * * * ?"
  }
}
```

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


## Features
Easy integration with IHostBuilder for background job processing.
Simple job registration using cron schedules.


## Issues
If you discover any issues, please open an issue on GitHub.

## License
This project is licensed under the MIT License - see the LICENSE file for details.