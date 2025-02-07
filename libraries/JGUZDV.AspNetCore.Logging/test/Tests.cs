using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace JGUZDV.AspNetCore.Logging.Test;

public class Tests
{

    [Fact]
    public void TestLoggingConfig()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Configuration
            .AddJsonFile("appsettings.test.json", optional: false)
            .AddEnvironmentVariables();

        builder.Configuration["Logging:File:OutputDirectory"] = Path.GetTempPath();
        builder.UseJGUZDVLogging(NullLogger.Instance);


        var app = builder.Build();

        var logger = app.Services.GetRequiredService<ILogger<Tests>>();
        logger.LogWarning("Warning!");
        logger.LogError("Error!");
    }
}