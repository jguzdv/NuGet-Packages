using System.Text;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace JGUZDV.Extensions.Logging.Tests;

public sealed class FileLoggingTests : IDisposable
{
    private readonly string _temporaryDirectory;

    public FileLoggingTests()
    {
        _temporaryDirectory = Path.Combine(
            Path.GetTempPath(),
            $"JGUZDV.Extensions.Logging.Tests-{Guid.NewGuid():N}");

        Directory.CreateDirectory(_temporaryDirectory);
    }

    [Fact]
    public void AddJGUZDVLogging_writes_to_two_configured_files()
    {
        const string message = "File logging integration test output";
        var firstFile = Path.Combine(_temporaryDirectory, "first.log");
        var secondFile = Path.Combine(_temporaryDirectory, "second.log");
        var configurationJson = $$"""
            {
              "Logging": {
                "File": {
                  "RootPath": "{{EscapeJson(_temporaryDirectory)}}",
                  "LogLevel": {
                    "Default": "Information"
                  },
                  "Files": [
                    { "Path": "first.log" },
                    { "Path": "second.log" }
                  ]
                }
              }
            }
            """;

        var configurationStream = new MemoryStream(Encoding.UTF8.GetBytes(configurationJson));
        using (configurationStream)
        {
            var builder = Host.CreateApplicationBuilder();
            builder.Configuration.AddJsonStream(configurationStream);
            builder.Logging.AddJGUZDVLogging(
                builder.Services,
                LoggingFileFormat.Json);

            using (var host = builder.Build())
            {
                var logger = host.Services.GetRequiredService<ILogger<FileLoggingTests>>();
                logger.LogInformation(message);
            }
        }

        Assert.True(File.Exists(firstFile));
        Assert.True(File.Exists(secondFile));
        Assert.Contains(message, File.ReadAllText(firstFile));
        Assert.Contains(message, File.ReadAllText(secondFile));
    }

    public void Dispose()
    {
        if (Directory.Exists(_temporaryDirectory))
            Directory.Delete(_temporaryDirectory, recursive: true);
    }

    private static string EscapeJson(string value)
    {
        return value.Replace("\\", "\\\\").Replace("\"", "\\\"");
    }
}
