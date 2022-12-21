using JGUZDV.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection;

public static class DataProtectionExtensions
{
    public static IDataProtectionBuilder AddZDVDataProtection(this IServiceCollection services, IConfigurationSection configurationSection, IWebHostEnvironment? environment = null) 
        => services.AddDataProtection().UseDataProtectionConfig(configurationSection, environment);

    public static IDataProtectionBuilder AddZDVDataProtection(this IServiceCollection services, IConfiguration config, IWebHostEnvironment? environment = null) 
        => services.AddZDVDataProtection(config.GetSection(Constants.DefaultSectionName), environment);

    public static IDataProtectionBuilder AddZDVDataProtection(this WebApplicationBuilder builder, string sectionName = Constants.DefaultSectionName) 
        => builder.Services.AddZDVDataProtection(builder.Configuration.GetSection(sectionName), builder.Environment);
}
