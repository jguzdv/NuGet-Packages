# JGUZDV.Extensions.Logging

It also adds the ability to log to files.
Currently the rollover is once a day.

```jsonc
{
  "Logging": {
    // Default Logging settings from AspNetCore
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    },

    "Console": {    // Overrides are supported
      "LogLevel": {
        "Default": Debug
      }
    }

    "File": {
      "Path": "/my-log-path/",    // Path where the log files should be written to
      "ApplicationName": "MyApp",     // Will be used if UseIsolatedPath is true
      "UseIsolatedPath": true,    // If true, resulting logfile will be placed in /my-log-path/MyApp/, default: true
      
      "FileName": "LogFile.log",    // Filename of the log, default: `Environment.MachineName + .json`, a date will automatically be added to the filename.
      "UseJson": true,    // Defaults to true, if filename ends with .json, will change the filename to .json, if true

      "LogLevel": {
        "Default": "Warning"
      }
    }
  }
}
```