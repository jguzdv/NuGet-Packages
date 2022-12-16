using JGUZDV.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection;

public static class DataProtectionExtensions
{
    public static IDataProtectionBuilder AddZDVDataProtectionConfig(this IServiceCollection services, IConfigurationSection configurationSection, IWebHostEnvironment? environment = null)
    {
        return services.AddDataProtection().UseDataProtectionConfig(configurationSection, environment);
    }

    public static IDataProtectionBuilder AddZDVDataProtectionConfig(this IServiceCollection services, IConfiguration config, IWebHostEnvironment? environment = null)
    {
        return services.AddZDVDataProtectionConfig(config.GetSection(Constants.DefaultSectionName), environment);
    }

    public static IDataProtectionBuilder AddZDVDataProtectionConfig(this WebApplicationBuilder builder)
    {
        return builder.Services.AddZDVDataProtectionConfig(builder.Configuration.GetSection(Constants.DefaultSectionName), builder.Environment);
    }
}
