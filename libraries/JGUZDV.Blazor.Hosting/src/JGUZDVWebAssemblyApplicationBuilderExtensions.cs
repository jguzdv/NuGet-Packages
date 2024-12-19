using System.Runtime.Versioning;

using JGUZDV.Blazor.Components.Localization;
using JGUZDV.Blazor.Hosting.Localization;

using Microsoft.Extensions.DependencyInjection;

namespace JGUZDV.AspNetCore.Hosting;

/// <summary>
/// Extension methods for the JGUZDVWebAssemblyApplicationBuilder.
/// </summary>
[SupportedOSPlatform("browser")]
public static class JGUZDVWebAssemblyApplicationBuilderExtensions
{
    /// <summary>
    /// Adds the required services for authorization.
    /// AuthenticationState will be handled via AuthenticationStateDeserialization.
    /// </summary>
    public static JGUZDVWebAssemblyApplicationBuilder AddAuthoriztion(this JGUZDVWebAssemblyApplicationBuilder appBuilder)
    {
        appBuilder.Services.AddAuthorizationCore();
        appBuilder.Services.AddCascadingAuthenticationState();
        appBuilder.Services.AddAuthenticationStateDeserialization();

        return appBuilder;
    }


    /// <summary>
    /// Adds the required services for localization.
    /// The current language and allowed languages will be handled via LocalizationStateDeserialization.
    /// </summary>
    public static JGUZDVWebAssemblyApplicationBuilder AddLocalization(this JGUZDVWebAssemblyApplicationBuilder appBuilder)
    {
        appBuilder.Services.AddLocalization();
        appBuilder.Services.AddScoped<ILanguageService, PersistentStateLanguageService>();

        return appBuilder;
    }
}
