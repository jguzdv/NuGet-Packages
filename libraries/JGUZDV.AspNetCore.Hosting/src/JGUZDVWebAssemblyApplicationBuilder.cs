using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace JGUZDV.AspNetCore.Hosting;

public class JGUZDVWebAssemblyApplicationBuilder
{
    private readonly WebAssemblyHostBuilder _webAssemblyHostBuilder;


    #region Forwarded Properties and Functions
    /// <summary>
    /// The WebApplicationBuilder for the application.
    /// </summary>
    public WebAssemblyHostBuilder Builder => _webAssemblyHostBuilder;

    /// <summary>
    /// Builds the WebApplication. <see cref="WebAssemblyHostBuilder.Build" />
    /// </summary>
    public WebAssemblyHost Build() => _webAssemblyHostBuilder.Build();

    /// <summary>
    /// The Configuration for the application. <see cref="WebAssemblyHostBuilder.Configuration" />
    /// </summary>
    public WebAssemblyHostConfiguration Configuration => _webAssemblyHostBuilder.Configuration;

    /// <summary>
    /// The Services for the application. <see cref="WebAssemblyHostBuilder.Services" />
    /// </summary>
    public IServiceCollection Services => _webAssemblyHostBuilder.Services;

    /// <summary>
    /// The Environment for the application. <see cref="WebAssemblyHostBuilder.HostEnvironment" />
    /// </summary>
    public IWebAssemblyHostEnvironment Environment => _webAssemblyHostBuilder.HostEnvironment;

    /// <summary>
    /// The LoggingBuilder for the application. <see cref="WebAssemblyHostBuilder.Logging" />
    /// </summary>
    public ILoggingBuilder Logging => _webAssemblyHostBuilder.Logging;
    #endregion


    private JGUZDVWebAssemblyApplicationBuilder(WebAssemblyHostBuilder webAssemblyHostBuilder)
    {
        _webAssemblyHostBuilder = webAssemblyHostBuilder;
    }

    public static JGUZDVWebAssemblyApplicationBuilder Create(WebAssemblyHostBuilder webAssemblyHostBuilder)
    {
        return new JGUZDVWebAssemblyApplicationBuilder(webAssemblyHostBuilder);
    }
}

public static class JGUZDVWebAssemblyApplicationBuilderExtensions
{
    public static JGUZDVWebAssemblyApplicationBuilder AddAuthoriztion(this JGUZDVWebAssemblyApplicationBuilder appBuilder)
    {
        appBuilder.Services.AddAuthorizationCore();
        appBuilder.Services.AddCascadingAuthenticationState();
        appBuilder.Services.AddAuthenticationStateDeserialization();

        return appBuilder;
    }

    public static JGUZDVWebAssemblyApplicationBuilder AddLocalization(this JGUZDVWebAssemblyApplicationBuilder appBuilder)
    {
        // TODO: Add localization services
        appBuilder.Services.AddLocalization();
        return appBuilder;
    }
}
