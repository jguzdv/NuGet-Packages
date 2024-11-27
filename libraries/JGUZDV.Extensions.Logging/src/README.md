# JGUZDV.Extensions.Logging

This package will configure file logging.
It will also add the ability to log to files.
Files will append the MachineName and automatically roll over once a day.

**Program.cs**
```csharp
builder.UseJGUZDVLogging();
```

**appsettings.json (minimal file configuration section)**
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
      "OutputDirectory": "/my-log-path/",    // Path where the log files should be written to

      "LogLevel": {
        "Default": "Warning"
      }
    }
  }
}
```