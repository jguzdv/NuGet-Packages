using JGUZDV.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for configuring data protection.
/// </summary>
public static class JGUZDVDataProtectionExtensions
{
    /// <summary>
    /// Adds data protection to the application using the specified configuration.
    /// </summary>
    public static IDataProtectionBuilder AddJGUZDVDataProtection(this IServiceCollection services, IConfigurationSection configurationSection, IWebHostEnvironment environment) 
        => services.AddDataProtection().UseDataProtectionConfig(configurationSection, environment);


    /// <summary>
    /// Adds data protection to the application using the specified configuration.
    /// </summary>
    public static IDataProtectionBuilder AddJGUZDVDataProtection(this IServiceCollection services, IConfiguration config, IWebHostEnvironment environment) 
        => services.AddJGUZDVDataProtection(config.GetSection(Constants.DefaultSectionName), environment);


    /// <summary>
    /// Adds data protection to the application using the specified configuration.
    /// </summary>
    public static IDataProtectionBuilder AddJGUZDVDataProtection(this WebApplicationBuilder builder, string sectionName = Constants.DefaultSectionName) 
        => builder.Services.AddJGUZDVDataProtection(builder.Configuration.GetSection(sectionName), builder.Environment);
}
