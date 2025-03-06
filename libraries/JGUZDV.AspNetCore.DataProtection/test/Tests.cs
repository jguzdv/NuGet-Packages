using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace JGUZDV.AspNetCore.DataProtection.Test;

public class Tests
{

    [Fact]
    public void TestDataProtectionConfig()
    {
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions { EnvironmentName = "test" });

        builder.AddJGUZDVDataProtection();

        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            var options = scope.ServiceProvider.GetRequiredService<IOptions<DataProtectionOptions>>().Value;
            Assert.Equal("TestApp", options.ApplicationDiscriminator);

            var dataProtection = scope.ServiceProvider.GetRequiredService<IDataProtectionProvider>();
            var protector = dataProtection.CreateProtector("Test");
            var protectedData = protector.Protect("Hello, World!");
            
            Assert.Equal("Hello, World!", protector.Unprotect(protectedData));
        }
    }
}