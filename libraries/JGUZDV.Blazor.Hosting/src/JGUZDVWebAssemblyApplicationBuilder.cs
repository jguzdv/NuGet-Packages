﻿using System.Globalization;
using System.Net;

using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;

namespace JGUZDV.Blazor.Hosting;

/// <summary>
/// A builder for configuring a WebAssembly application (aka Blazor client).
/// </summary>
public class JGUZDVWebAssemblyApplicationBuilder
{
    private readonly WebAssemblyHostBuilder _webAssemblyHostBuilder;

    #region Forwarded Properties and Functions
    /// <summary>
    /// The WebAssemblyHostBuilder for the application.
    /// </summary>
    public WebAssemblyHostBuilder Builder => _webAssemblyHostBuilder;

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
    public static JGUZDVWebAssemblyApplicationBuilder CreateDefault(string[]? args = null,
        Action<JGUZDVWebAssemblyApplicationBuilder>? configureConfiguration = null)
    {
        var builder = new JGUZDVWebAssemblyApplicationBuilder(WebAssemblyHostBuilder.CreateDefault(args));
        configureConfiguration?.Invoke(builder);

        builder.AddAuthoriztion();
        builder.AddLocalization();

        return builder;
    }


    /// <summary>
    /// Builds the WebAssemblyHost. <see cref="WebAssemblyHostBuilder.Build" />
    /// Additionally it will set the current culture to the documents lang.
    /// </summary>
    public async Task<WebAssemblyHost> BuildAsync()
    {
        var host = _webAssemblyHostBuilder.Build();

        await ReadAndSetCultureFromHtmlTag(host);

        return host;
    }

    /// <summary>
    /// Reads the culture from the html tag and sets it as the current culture.
    /// </summary>
    public async Task ReadAndSetCultureFromHtmlTag(WebAssemblyHost host, string defaultCulture = "de-DE")
    {
        var js = host.Services.GetRequiredService<IJSRuntime>();
        var cultureName = defaultCulture;
        var uiCultureName = defaultCulture;
        try
        {
            var allCookies = await js.InvokeAsync<string>("document.getCookies");
            var cookies = WebUtility.UrlDecode(allCookies);

            var cultureCookieValue = cookies.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(c => c.Split('=', 2, StringSplitOptions.TrimEntries))
                // from all cookies take the last matching
                .LastOrDefault(c => c[0] == ".AspNetCore.Culture")?
                // and take the value
                .LastOrDefault();

            if (cultureCookieValue != null)
            {
                var cultures = cultureCookieValue.Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                foreach(var c in cultures)
                {
                    if(c.Split('=', 2, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries) is [string name, string value])
                    {
                        if(name == "c")
                        {
                            cultureName = value;
                        }
                        else if (name == "uic")
                        {
                            uiCultureName = value;
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Failed to read culture from cookie: " + ex.Message);
        }

        
        CultureInfo.DefaultThreadCurrentCulture = CultureInfo.GetCultureInfo(cultureName);
        CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo(uiCultureName);
    }
}
