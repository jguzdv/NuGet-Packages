﻿using JGUZDV.AspNetCore.Hosting.FeatureManagement;

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

    #region Forwarded Properties and Functions
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
    #endregion


    #region Configuration flags
    /// <summary>
    /// Gets a value indicating whether the application has authentication configured.
    /// </summary>
    public bool HasAuthentication { get; internal set; }

    /// <summary>
    /// Gets a value indicating whether the application has request localization configured.
    /// </summary>
    public bool HasRequestLocalization { get; internal set; }


    /// <summary>
    /// Gets a value indicating whether the application has feature management configured.
    /// </summary>
    public bool HasFeatureManagement { get; internal set; }


    /// <summary>
    /// Gets a value indicating whether the application has health checks configured.
    /// </summary>
    public bool HasHealthChecks { get; internal set; }


    /// <summary>
    /// Gets a value indicating whether the application has telemetry configured.
    /// </summary>
    public bool HasOpenTelemetry { get; internal set; }


    /// <summary>
    /// Gets a value indicating whether the application has open api configured.
    /// </summary>
    public bool HasOpenApi { get; internal set; }


    /// <summary>
    /// Gets a value indicating whether the application has mvc configured.
    /// </summary>
    public bool HasMVC { get; internal set; }


    /// <summary>
    /// Gets a value indicating whether the application has razor pages configured.
    /// </summary>
    public bool HasRazorPages { get; internal set; }


    /// <summary>
    /// Gets a value indicating whether the application has razor components configured. (Blazor Static Server)
    /// </summary>
    public bool HasRazorComponents { get; internal set; }

    /// <summary>
    /// Gets a value indicating whether the application has interactive server components configured (Blazor Static Server + Interactive Client)
    /// </summary>
    public bool HasInteractiveServerComponents { get; internal set; }


    /// <summary>
    /// Gets a value indicating whether the application has a frontend configured.
    /// </summary>
    public bool HasFrontend { get; internal set; }
    #endregion


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



    /// <summary>
    /// Configures the application and maps routes depending on the added services (marked with *).<br />
    /// The following mappings will occur (in order):<br />
    /// - StaticFiles<br />
    /// - Routing<br />
    /// - OpenApi*<br />
    /// - Telemetry*<br />
    /// - Authentication and Authorization*<br />
    /// - RequestLocalization*<br />
    /// - FeatureManagement Endpoints*<br />
    /// - MVC Controllers*<br />
    /// - Razor Pages*<br />
    /// </summary>
    public WebApplication BuildAndConfigureDefault()
    {
        var app = _webApplicationBuilder.Build();

        return ConfigureDefaultApp(app);
    }


    /// <summary>
    /// Configures the application and maps routes for a blazor enabled web host.<br />
    /// It maps static assets and the razor components.<br />
    /// See <see cref="BuildAndConfigureDefault"/> for further mappings.
    /// </summary>
    /// <typeparam name="TRootComponent">The root component for blazor.</typeparam>
    /// <param name="additionalBlazorAssemblies">Additional assemblies containing pages for blazor.</param>
    public WebApplication BuilderAndConfigureBlazor<TRootComponent>(params System.Reflection.Assembly[] additionalBlazorAssemblies)
    {
        var app = _webApplicationBuilder.Build();

        if (HasRazorComponents)
        {
            app.MapStaticAssets();
        }

        ConfigureDefaultApp(app);

        if (HasRazorComponents)
        {
            var b = app.MapRazorComponents<TRootComponent>();
            b.AddAdditionalAssemblies(additionalBlazorAssemblies);

            if (HasInteractiveServerComponents) {
                b.AddInteractiveServerRenderMode();
            }
        }

        return app;
    }


    private WebApplication ConfigureDefaultApp(WebApplication app)
    {
        app.UseStaticFiles();

        app.UseRouting();

        if (HasOpenApi)
        {
            app.MapOpenApi();
        }

        if (HasHealthChecks)
        {
            app.MapHealthChecks("/health");
        }


        if (HasOpenTelemetry)
        {
            // TODO AuthN/AuthZ?
            // TODO app.MapPrometheus();
        }


        if (HasAuthentication)
        {
            app.UseAuthentication();
            app.UseAuthorization();
        }

        if (HasRequestLocalization)
        {
            app.UseRequestLocalization();
        }

        if (HasFrontend)
        {
            app.UseAntiforgery();
        }

        if (HasFeatureManagement)
        {
            // Maps featue management to /_app/features
            app.MapFeatureManagement();
        }

        if (HasMVC)
        {
            app.MapControllers();
        }

        if (HasRazorPages)
        {
            app.MapRazorPages();
        }

        return app;
    }
}