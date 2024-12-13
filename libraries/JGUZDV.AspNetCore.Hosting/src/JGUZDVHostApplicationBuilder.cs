using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace JGUZDV.AspNetCore.Hosting;

/// <summary>
/// Wraps the WebApplicationBuilder for the application.
/// </summary>
public class JGUZDVHostApplicationBuilder
{
    private readonly WebApplicationBuilder _webApplicationBuilder;

    /// <summary>
    /// The WebApplicationBuilder for the application.
    /// </summary>
    public WebApplicationBuilder Builder => _webApplicationBuilder;

    /// <summary>
    /// Builds the WebApplication. <see cref="WebApplicationBuilder.Build" />
    /// </summary>
    public WebApplication Build() => _webApplicationBuilder.Build();

    /// <summary>
    /// The Configuration for the application. <see cref="WebApplicationBuilder.Configuration" />
    /// </summary>
    public ConfigurationManager Configuration => _webApplicationBuilder.Configuration;

    /// <summary>
    /// The Services for the application. <see cref="WebApplicationBuilder.Services" />
    /// </summary>
    public IServiceCollection Services => _webApplicationBuilder.Services;

    /// <summary>
    /// The Environment for the application. <see cref="WebApplicationBuilder.Environment" />
    /// </summary>
    public IWebHostEnvironment Environment => _webApplicationBuilder.Environment;

    /// <summary>
    /// The LoggingBuilder for the application. <see cref="WebApplicationBuilder.Logging" />
    /// </summary>
    public ILoggingBuilder Logging => _webApplicationBuilder.Logging;


    private JGUZDVHostApplicationBuilder(WebApplicationBuilder webAppBuilder)
    {
        _webApplicationBuilder = webAppBuilder;
    }

    /// <summary>
    /// Creates a new JGUZDVApplicationBuilder, that wraps an WebApplicationBuilder
    /// </summary>
    public static JGUZDVHostApplicationBuilder Create(string[] args)
    {
        return new JGUZDVHostApplicationBuilder(
            WebApplication.CreateBuilder(args)
            );
    }



    public WebApplication BuildAndConfigureDefault()
    {
        var app = _webApplicationBuilder.Build();

        app.UseStaticFiles();

        app.UseRouting();

        if (HasAuthentication)
        {
            app.UseAuthentication();
            app.UseAuthorization();
        }

        if (HasRequestLocalization)
        {
            app.UseRequestLocalization();
        }

        // TODO
        //if (HasTelemetry)
        //{
        //    app.UseTelemetry();
        //}

        if(HasOpenApi)
        {
            app.MapOpenApi();
        }

        return app;
    }


    public bool HasAuthentication { get; internal set; }
    public bool HasRequestLocalization { get; internal set; }
    public bool HasFeatureManagement { get; internal set; }
    public bool HasTelemetry { get; internal set; }
    public bool HasOpenApi { get; internal set; }
}
