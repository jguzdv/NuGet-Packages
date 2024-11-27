# JGUZDV.Extensions.Logging.File

Adds a LoggingProvider for files. Usage:

```csharp
builder.Logging.AddJsonFile();
```

**appsettings.json (Smallest possible configuration required)**
```jsonc
{
  "Logging": {
    "File": {
      "OutputDirectory": "/my-log-path/",    // Path where the log files should be written to
    }
  }
}
```