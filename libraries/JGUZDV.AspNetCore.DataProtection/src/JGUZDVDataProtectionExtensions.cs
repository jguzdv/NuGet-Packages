using JGUZDV.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection;

public static class JGUZDVDataProtectionExtensions
{
    public static IDataProtectionBuilder AddJGUZDVDataProtection(this IServiceCollection services, IConfigurationSection configurationSection, IWebHostEnvironment? environment = null) 
        => services.AddDataProtection().UseDataProtectionConfig(configurationSection, environment);

    public static IDataProtectionBuilder AddJGUZDVDataProtection(this IServiceCollection services, IConfiguration config, IWebHostEnvironment? environment = null) 
        => services.AddJGUZDVDataProtection(config.GetSection(Constants.DefaultSectionName), environment);

    public static IDataProtectionBuilder AddJGUZDVDataProtection(this WebApplicationBuilder builder, string sectionName = Constants.DefaultSectionName) 
        => builder.Services.AddJGUZDVDataProtection(builder.Configuration.GetSection(sectionName), builder.Environment);
}
