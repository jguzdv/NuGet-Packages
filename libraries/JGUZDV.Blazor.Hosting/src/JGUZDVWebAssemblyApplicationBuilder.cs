using System.Runtime.Versioning;

using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace JGUZDV.AspNetCore.Hosting;

/// <summary>
/// A builder for configuring a WebAssembly application (aka Blazor client).
/// </summary>
[SupportedOSPlatform("browser")]
public class JGUZDVWebAssemblyApplicationBuilder
{
    private readonly WebAssemblyHostBuilder _webAssemblyHostBuilder;

    #region Forwarded Properties and Functions
    /// <summary>
    /// The WebAssemblyHostBuilder for the application.
    /// </summary>
    public WebAssemblyHostBuilder Builder => _webAssemblyHostBuilder;

    /// <summary>
    /// Builds the WebAssemblyHost. <see cref="WebAssemblyHostBuilder.Build" />
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


    /// <summary>
    /// Creates a new instance of the JGUZDVWebAssemblyApplicationBuilder, that wraps a default WebAssemblyHostBuilder.
    /// </summary>
    public static JGUZDVWebAssemblyApplicationBuilder Create(string[]? args = null)
    {
        return new JGUZDVWebAssemblyApplicationBuilder(WebAssemblyHostBuilder.CreateDefault(args));
    }


    /// <summary>
    /// Create a new instance of the JGUZDVWebAssemblyApplicationBuilder, that wraps a default WebAssemblyHostBuilder and adds the default services.
    /// Default services are:<br />
    /// - Authorization<br />
    /// - Localization
    /// </summary>
    public static JGUZDVWebAssemblyApplicationBuilder CreateDefault(string[]? args = null)
    {
        var builder = new JGUZDVWebAssemblyApplicationBuilder(WebAssemblyHostBuilder.CreateDefault(args));

        builder.AddAuthoriztion();
        builder.AddLocalization();

        return builder;
    }
}
