using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace JGUZDV.AspNetCore.DataProtection.Test;

public class Tests
{

    [Fact]
    public void TestDataProtectionConfig()
    {
        
        var builder = WebApplication.CreateBuilder();

        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.test.json")
            .AddEnvironmentVariables()
            .Build();

        builder.Services
            .AddDataProtection()
            .UseDataProtectionConfig(config.GetSection("DataProtection"), builder.Environment);
    }
}