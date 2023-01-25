using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace JGUZDV.Extensions.Logging.Test;

public class Tests
{

    [Fact]
    public void TestLoggingConfig()
    {
        
        var builder = Host.CreateDefaultBuilder();
        builder.ConfigureAppConfiguration(c => c
            .AddJsonFile("appsettings.test.json", optional: false)
            .AddEnvironmentVariables()
        );

        builder.UseJGUZDVLogging();


        var app = builder.Build();

        var logger = app.Services.GetRequiredService<ILogger<Tests>>();
        logger.LogWarning("Warning!");
        logger.LogError("Error!");
    }
}