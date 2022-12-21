using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace JGUZDV.AspNetCore.Logging.Test;

public class Tests
{

    [Fact]
    public void TestLoggingConfig()
    {
        
        var builder = WebApplication.CreateBuilder();
        builder.Configuration
            .AddJsonFile("appsettings.test.json", optional: false)
            .AddEnvironmentVariables()
            .Build();

        builder.UseZDVLogging();


        var app = builder.Build();

        var logger = app.Services.GetRequiredService<ILogger<Tests>>();
        logger.LogWarning("Warning!");
        logger.LogError("Error!");
    }
}