using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace JGUZDV.AspNetCore.DataProtection.Test;

public class Tests
{

    [Fact]
    public void TestDataProtectionConfig()
    {
        
        var builder = WebApplication.CreateBuilder();
        builder.Configuration
            .AddJsonFile("appsettings.test.json")
            .AddEnvironmentVariables()
            .Build();

        builder.AddZDVDataProtection();
    }
}