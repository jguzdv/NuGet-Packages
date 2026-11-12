# JGUZDV.Extensions.Logging

This package adds file logging through
[Karambolo.Extensions.Logging.File.Json](https://github.com/adams85/filelogger).
The logger can write JSON or plain-text files and supports date, machine name,
application name, and discriminator placeholders in file paths.

**Program.cs**
```csharp
builder.Logging.AddJGUZDVLogging(
    builder.Services,
    builder.Configuration,
    LoggingFileFormat.Json);
```

Use `LoggingFileFormat.PlainText` instead to write plain-text log files.

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
        "Default": "Debug"
      }
    },

    "File": {
      "LogLevel": {
        "Default": "Warning"
      },
      "Files": [
        {
          "Path": "logs/<applicationname>/<date:yyyy-MM-dd>-<machinename>.log"
        }
      ]
    }
  }
}
```

At least one entry is required in `Logging:File:Files`. The path may use the
following placeholders in addition to the file logger's built-in placeholders:

- `<applicationname>` — the host application name.
- `<discriminator>` — the value of `Logging:File:Discriminator`.
- `<machinename>` — the machine name.