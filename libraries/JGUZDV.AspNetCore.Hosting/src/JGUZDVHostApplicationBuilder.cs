﻿using System.Runtime.Versioning;

using JGUZDV.AspNetCore.Hosting.Authentication;
using JGUZDV.AspNetCore.Hosting.Components;
using JGUZDV.AspNetCore.Hosting.Configuration;
using JGUZDV.AspNetCore.Hosting.Diagnostics;
using JGUZDV.AspNetCore.Hosting.Extensions;
using JGUZDV.AspNetCore.Hosting.FeatureManagement;
using JGUZDV.AspNetCore.Hosting.Localization;
using JGUZDV.AspNetCore.Hosting.Resources;
using JGUZDV.YARP.SimpleReverseProxy;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace JGUZDV.AspNetCore.Hosting;

/// <summary>
/// Wraps the WebApplicationBuilder for the application.
/// </summary>
[UnsupportedOSPlatform("browser")]
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
    /// Gets a value indicating whether the application has session configured.
    /// </summary>
    public bool HasSession { get; internal set; }


    /// <summary>
    /// Gets a value indicating whether the application has feature management configured.
    /// </summary>
    public bool HasFeatureManagement { get; internal set; }


    /// <summary>
    /// Gets a value indicating whether the application has forwarded headers configured.
    /// </summary>
    public bool HasForwardedHeaders { get; internal set; }


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
    /// Gets a value indicating whether the application has interactive server components configured (Blazor Server)
    /// </summary>
    public bool HasInteractiveServerComponents { get; internal set; }

    /// <summary>
    /// Gets a value indicating whether the application has interactive web assembly components configured (Blazor WebAssembly)
    /// </summary>
    public bool HasInteractiveWebAssemblyComponents { get; internal set; }


    /// <summary>
    /// Gets a value indicating whether the application has a frontend configured.
    /// </summary>
    public bool HasFrontend { get; internal set; }


    /// <summary>
    /// Gets a value indicating whether the application has a reverse proxy configured.
    /// </summary>
    public bool HasReverseProxy { get; internal set; }
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
    /// Creates a new JGUZDVApplicationBuilder, that is preconfigured as WebApi Host. The following services are configured (* only if found in config):<br />
    /// - Logging<br />
    /// - ProblemDetails<br />
    /// - OpenApi<br />
    /// - Json-Defaults for Minimal-Api<br />
    /// - RequestLocalization<br />
    /// - DistributedCache* when in Production or DistributedInMemoryCache<br />
    /// - DataProtection* when in Production or ephemeral DataProtection<br />
    /// - OpenIdConnect Authentication*<br />
    /// - FeatureManagement*<br />
    /// - ForwarededHeaders*<br />
    /// - OpenTelemetry*<br />
    /// - HealthChecks<br />
    /// - ReverseProxy*<br />
    /// - MVC (with view support)<br />
    /// - RazorPages<br />
    /// - Razor WebComponents (without interactivity aka. Blazor Static Server)<br />
    /// </summary>
    public static JGUZDVHostApplicationBuilder CreateWebHost(string[] args,
        BlazorInteractivityModes interactivityMode = BlazorInteractivityModes.DisableBlazor,
        Action<JGUZDVHostApplicationBuilder>? configureConfiguration = null)
    {
        var builder = Create(args);
        configureConfiguration?.Invoke(builder);

        builder.AddWebHostServices(interactivityMode);

        return builder;
    }


    /// <summary>
    /// Adds the services for a Web Host.<br />
    /// See <see cref="CreateWebHost"/> for further details.
    /// </summary>
    public void AddWebHostServices(BlazorInteractivityModes interactivityMode)
    {
        AddMachineConfigIfExists();
        AddLogging();

        // We're rebuilding the service provider to enable logging during log setup including the log created just before.
        using (var sp = Services.BuildServiceProvider())
        using (var loggerFactory = sp.GetRequiredService<ILoggerFactory>())
        {
            var logger = loggerFactory.CreateLogger(nameof(JGUZDVHostApplicationBuilder));
            var missingConfigLogLevel = Environment.IsProduction() ? LogLevel.Information : LogLevel.Warning;

            TryAddForwardedHeaders(logger);
            TryAddDataProtection(logger);
            TryAddDistributedCache(logger);
            TryAddFeatureManagement(logger);
            TryAddOpenTelemetry(logger);


            Services.AddProblemDetails();
            LogMessages.FeatureAdded(logger, "ProblemDetails");

            // TODO: Enable swagger UI?
            this.AddOpenApi();
            LogMessages.FeatureAdded(logger, "OpenApi");

            this.AddLocalization(logger: logger);
            LogMessages.FeatureAdded(logger, "RequestLocalization");

            this.AddHealthChecks();
            LogMessages.FeatureAdded(logger, "HealthChecks");



            // OpenId Connect Authentication
            if (Configuration.HasConfigSection(Constants.ConfigSections.OpenIdConnectAuthentication))
            {
                this.AddDefaultOpenIdConnectAuthentication(logger: logger);
                LogMessages.FeatureConfigured(logger, "OpenIdConnectAuthentication", Constants.ConfigSections.OpenIdConnectAuthentication);
            }
            else
            {
                LogMessages.MissingOptionalConfig(logger, "OpenIdConnectAuthentication", Constants.ConfigSections.OpenIdConnectAuthentication);
            }


            


            // Reverse Proxy
            if (Configuration.HasConfigSection(Constants.ConfigSections.ReverseProxy))
            {
                this.AddReverseProxy();
                LogMessages.FeatureConfigured(logger, "ReverseProxy", Constants.ConfigSections.ReverseProxy);
            }
            else
            {
                LogMessages.MissingOptionalConfig(logger, "ReverseProxy", Constants.ConfigSections.ReverseProxy);
            }


            // Frontend Frameworks
            this.AddAspNetCoreMvc(enableViewSupport: true);
            LogMessages.FeatureAdded(logger, "MVC with View support");

            this.AddRazorPages();
            LogMessages.FeatureAdded(logger, "RazorPages");

            if (interactivityMode != BlazorInteractivityModes.DisableBlazor)
            {
                this.AddBlazor(interactivityMode, logger: logger);
            }
        }
    }


    /// <summary>
    /// Creates a new JGUZDVApplicationBuilder, that is preconfigured as WebApi Host. The following services are configured (* only if found in config):<br />
    /// - Logging<br />
    /// - ProblemDetails<br />
    /// - OpenApi<br />
    /// - Json-Defaults for Minimal-Api<br />
    /// - RequestLocalization<br />
    /// - DistributedCache* when in Production or DistributedInMemoryCache<br />
    /// - DataProtection* when in Production or ephemeral DataProtection<br />
    /// - OpenIdConnect Authentication*<br />
    /// - FeatureManagement*<br />
    /// - ForwardedHeaders*<br />
    /// - OpenTelemetry*<br />
    /// - HealthChecks<br />
    /// - ReverseProxy*<br />
    /// - MVC (with view support)<br />
    /// - RazorPages<br />
    /// - Razor WebComponents (without interactivity aka. Blazor Static Server)<br />
    /// </summary>
    public static JGUZDVHostApplicationBuilder CreateStaticWeb(string[] args, Action<JGUZDVHostApplicationBuilder>? configureConfiguration = null)
        => CreateWebHost(args, BlazorInteractivityModes.None, configureConfiguration);


    /// <summary>
    /// Creates a new JGUZDVApplicationBuilder, that is preconfigured as WebApi Host. The following services are configured (* only if found in config):<br />
    /// - Logging<br />
    /// - ProblemDetails<br />
    /// - OpenApi<br />
    /// - MVC (without view support)<br />
    /// - Json-Defaults for Minimal-Api<br />
    /// - RequestLocalization<br />
    /// - DistributedCache* when in Production or DistributedInMemoryCache<br />
    /// - DataProtection* when in Production or ephemeral DataProtection<br />
    /// - JwtBearerAuthentication*<br />
    /// - FeatureManagement*<br />
    /// - OpenTelemetry*<br />
    /// - HealthChecks<br />
    /// </summary>
    public static JGUZDVHostApplicationBuilder CreateWebApi(string[] args,
       Action<JGUZDVHostApplicationBuilder>? configureConfiguration = null)
    {
        var builder = Create(args);
        configureConfiguration?.Invoke(builder);

        builder.AddWebApiServices();

        return builder;
    }

    /// <summary>
    /// Adds the services for a WebApi Host.<br />
    /// See <see cref="CreateWebApi"/> for further details.
    /// </summary>
    public void AddWebApiServices()
    {
        AddMachineConfigIfExists();
        AddLogging();

        using (var sp = Services.BuildServiceProvider())
        using (var loggerFactory = sp.GetRequiredService<ILoggerFactory>())
        {
            var logger = loggerFactory.CreateLogger(nameof(JGUZDVHostApplicationBuilder));
            var missingConfigLogLevel = Environment.IsProduction() ? LogLevel.Information : LogLevel.Warning;


            TryAddForwardedHeaders(logger);
            TryAddDataProtection(logger);
            TryAddDistributedCache(logger);
            TryAddFeatureManagement(logger);
            TryAddOpenTelemetry(logger);


            Services.AddProblemDetails();
            LogMessages.FeatureAdded(logger, "ProblemDetails");

            // TODO: Enable swagger UI?
            this.AddOpenApi();
            LogMessages.FeatureAdded(logger, "OpenApi");

            this.AddAspNetCoreMvc(enableViewSupport: false);
            Services.ConfigureHttpJsonOptions(opt => opt.ApplyJsonOptions());
            LogMessages.FeatureAdded(logger, "MVC without View support");

            this.AddLocalization(logger: logger);
            LogMessages.FeatureAdded(logger, "RequestLocalization");






            if (Configuration.HasConfigSection(Constants.ConfigSections.JwtBearerAuthentication))
            {
                this.AddDefaultJwtBearerAuthentication(logger: logger);
                LogMessages.FeatureConfigured(logger, "JwtBearerAuthentication", Constants.ConfigSections.JwtBearerAuthentication);
            }
            else
            {
                LogMessages.MissingOptionalConfig(logger, "JwtBearerAuthentication", Constants.ConfigSections.JwtBearerAuthentication);
            }


            this.AddHealthChecks();
            LogMessages.FeatureAdded(logger, "HealthChecks");
        }
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
    public WebApplication BuildAndConfigureBlazor<TRootComponent>(params System.Reflection.Assembly[] additionalBlazorAssemblies)
    {
        var app = _webApplicationBuilder.Build();

        if (HasRazorComponents)
        {
            app.MapStaticAssets();
        }

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseWebAssemblyDebugging();
        }

        ConfigureDefaultApp(app);

        if (HasRazorComponents)
        {
            var b = app.MapRazorComponents<TRootComponent>();
            b.AddAdditionalAssemblies(additionalBlazorAssemblies);

            if (HasInteractiveServerComponents)
            {
                b.AddInteractiveServerRenderMode();
            }

            if (HasInteractiveWebAssemblyComponents)
            {
                b.AddInteractiveWebAssemblyRenderMode();
            }
        }

        return app;
    }

    /// <summary>
    /// The method is a static method that generates an HTML error page based on the provided status code, localizer, and HTTP context information.
    /// </summary>
    /// <param name="localizer">An <see cref="IStringLocalizer"/> instance used to retrieve localized strings for the error page.</param>
    /// <param name="statusCode">The HTTP status code representing the error condition. Determines the title and message displayed on the error
    /// page.</param>
    /// <param name="context">The <see cref="HttpContext"/> containing request-specific information, such as the trace identifier, request
    /// path, and query string.</param>
    public static string GenerateErrorPageHtml(
            IStringLocalizer localizer,
            int statusCode,
            HttpContext context)
    {
        var request = context.Request;
        
        var title = localizer[$"Title.{statusCode}"];
        var message = localizer[$"Message.{statusCode}"];

        if (title.ResourceNotFound) {
            title = localizer["Title.Default"];
        }
        if (message.ResourceNotFound) {
            message = localizer["Message.Default"];
        }

        return $"""
                <html>
                    <head> 
                        <title>{title}</title>
                        <meta charset="utf-8" />

                        <link rel="icon" href="https://cdn.zdv.uni-mainz.de/web/assets/JGU-Quader.ico">
                        <link rel="icon" sizes="16x16 32x32 64x64" href="https://cdn.zdv.uni-mainz.de/web/assets/JGU-Quader.ico">
                        <link rel="icon" sizes="180x180" href="https://cdn.zdv.uni-mainz.de/web/assets/JGU-Quader-180.png">

                        <link href="https://cdn.zdv.uni-mainz.de/web/jg-ootstrap/jg-ootstrap.css" rel="stylesheet" />
                        <link href="https://cdn.zdv.uni-mainz.de/web/fontawesome/5-free/css/all.min.css" rel="stylesheet" />
                    </head>
                    <body>
                        <div class="container col-xl-10 col-xxl-8 px-4 py-5">
                            <div class="row align-items-center g-lg-5 py-5">
                                <div class="col-lg-7 text-center text-lg-start">
                                    <h1 class="display-4 fw-bold lh-1 mb-3">{title}</h1>
                                    <p class="col-lg-10 fs-4">{message}</p>
                                </div>
                                <div class="col-md-10 mx-auto col-lg-5"> 
                                    <div class="card">
                                      <div class="card-body p-4 bg-light">
                                        <h4 class="card-title d-flex justify-content-between align-items-center">{localizer["Errorinformations"]} <i class="fas fa-info fs-5"></i></h4>
                                        <h6 class="card-subtitle mb-2 text-body-secondary">{localizer["Technical.Hint"]}</h6>
                                        <ul class="list-group pt-2">
                                          <li class="list-group-item bg-light"><strong>Trace ID:</strong> {context.TraceIdentifier}</li>
                                          <li class="list-group-item bg-light"><strong>{localizer["Time"]}:</strong> {DateTimeOffset.UtcNow:O}</li>
                                          <li class="list-group-item bg-light"><strong>Host:</strong> {request.Host}</li>
                                          <li class="list-group-item bg-light"><strong>URL:</strong> {request.Path}{request.QueryString}</li>
                                        </ul>
                                      </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </body>
                </html>
                """;
    }


    /// <summary>
    /// Configures the application and maps routes as configured*, for frontend only+:<br />
    /// - StaticFiles<br />
    /// - Forwarded Headers*<br />
    /// - Error Handling*+<br />
    /// - Https Redirection*+<br />
    /// - Session*<br />
    /// - Routing<br />
    /// - OpenApi*<br />
    /// - Health Checks*<br />
    /// - Open Telemetry*<br />
    /// - Authentication and Authorization*<br />
    /// - Request Localization*<br />
    /// - PersistentComponentState Middleware*<br />
    /// - Reverse Proxy*<br />
    /// - Anti Forgery*+<br />
    /// - Authentication Endpoints*+<br />
    /// - Language Selection Endpoints*+<br />
    /// - Feature Management Endpoints*<br />
    /// - Mvc Controllers*<br />
    /// - Razor Pages*<br />
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    private WebApplication ConfigureDefaultApp(WebApplication app)
    {
        app.UseStaticFiles();

        if (HasForwardedHeaders)
        {
            app.UseForwardedHeaders();
        }

        if (HasFrontend)
        {
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error", createScopeForErrors: true);
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            else
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStatusCodePages(async context =>
            {
                var statusCode = context.HttpContext.Response.StatusCode;
                var localizer = context.HttpContext.RequestServices.GetRequiredService<IStringLocalizer<ErrorResource>>();

                var html = GenerateErrorPageHtml(localizer, statusCode, context.HttpContext);

                context.HttpContext.Response.ContentType = "text/html";
                context.HttpContext.Response.Headers.ContentLength = null;
                await context.HttpContext.Response.WriteAsync(html);
            });

            app.UseHttpsRedirection();
        }

        if (HasSession)
        {
            app.UseSession();
        }

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

        if (HasInteractiveWebAssemblyComponents)
        {
            // In Blazor WebAssembly, we'll use this to serialize settings and other application state (e.g. RequestLocalization Options) to the client.
            app.UseMiddleware<PersistentComponentStateMiddleware>();
        }

        if (HasReverseProxy)
        {
            app.MapSimpleReverseProxy();
        }

        if (HasFrontend)
        {
            app.UseAntiforgery();

            if (HasAuthentication)
            {
                // Maps login and logout to /_app/sing-in and /_app/sign-out
                app.MapAuthentication();
            }
        }

        if (HasRequestLocalization)
        {
            // Maps language select to /_app/localization
            app.MapLocalization();
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

        app.MapDiagnostics();

        return app;
    }


    /// <summary>
    /// Adds default logging services to the application.
    /// This will build the service provider and dispose it afterwards, so we can log the logging builder.
    /// </summary>
    public void AddLogging()
    {
        // Were building the service provider to enable logging during log setup.
        // While this seems counterintuitive, it is necessary to log missing configurations.
        // At this point, AspNetCore will already have setup logging to console, EventLog and Debug.
        using (var sp = Services.BuildServiceProvider())
        using (var loggerFactory = sp.GetRequiredService<ILoggerFactory>())
        {
            var logger = loggerFactory.CreateLogger(nameof(JGUZDVHostApplicationBuilder));
            this.AddLogging(logger);
        }
    }

    /// <summary>
    /// Adds a global configuration file to the config, that's either named "MachineConfig" or "JGUZDV:MachineConfig".
    /// </summary>
    public void AddMachineConfigIfExists()
    {
        // Were building the service provider to enable logging during log setup.
        // While this seems counterintuitive, it is necessary to log missing configurations.
        // At this point, AspNetCore will already have setup logging to console, EventLog and Debug.
        using (var sp = Services.BuildServiceProvider())
        using (var loggerFactory = sp.GetRequiredService<ILoggerFactory>())
        {
            var logger = loggerFactory.CreateLogger(nameof(JGUZDVHostApplicationBuilder));

            var machineConfigFile = Configuration["JGUZDV:MachineConfig"] ?? Configuration["MachineConfig"];
            if (!string.IsNullOrWhiteSpace(machineConfigFile))
            {
                Configuration.AddAsFirstJsonFile(machineConfigFile, optional: true, reloadOnChange: true);
                LogMessages.MachineConfigurationFileAdded(logger, machineConfigFile);
            }
            else
            {
                LogMessages.MissingOptionalConfig(logger, "MachineConfig", "MachineConfig");
            }
        }
    }


    /// <summary>
    /// Adds ForwardedHeaders services from config section 'ForwardedHeaders' to the application, if the section exists.
    /// </summary>
    public void TryAddForwardedHeaders(ILogger logger)
    {
        if (Configuration.HasConfigSection(Constants.ConfigSections.ForwardedHeaders))
        {
            this.AddForwardedHeaders();
            LogMessages.FeatureConfigured(logger, "ForwardedHeaders", Constants.ConfigSections.ForwardedHeaders);
        }
        else
        {
            LogMessages.ForwardedHeadersMissing(logger, Constants.ConfigSections.ForwardedHeaders);
        }
    }


    /// <summary>
    /// Adds DataProtection services from config section 'JGUZDV:DataProtection' to the application, if the section exists.<br />
    /// In development, the application will use ephemeral data protection.<br />
    /// This will log a critical message if the application discriminator is not set.
    /// </summary>
    public void TryAddDataProtection(ILogger logger)
    {
        var hasDataProtectionSection = Configuration.HasConfigSection(Constants.ConfigSections.DataProtection);
        if (Environment.IsProduction() && hasDataProtectionSection)
        {
            this.AddDataProtection();
            LogMessages.FeatureConfigured(logger, "DataProtection", Constants.ConfigSections.DataProtection);
        }
        else
        {
            if (!hasDataProtectionSection)
            {
                LogMessages.DataProtectionMissing(logger, Constants.ConfigSections.DataProtection);
            }

            if (string.IsNullOrWhiteSpace(Configuration[$"{Constants.ConfigSections.DataProtection}:ApplicationDiscriminator"]))
            {
                LogMessages.ApplicationDiscriminatorNotSet(logger, $"{Constants.ConfigSections.DataProtection}:ApplicationDiscriminator", Environment.ApplicationName);
            }

            Services.AddDataProtection();
            LogMessages.FeatureAdded(logger, "EphemeralDataProtection");
        }
    }

    /// <summary>
    /// Adds DistributedCache services from config section 'DistributedCache' to the application, if the section exists.<br />
    /// In development, the application will use DistributedMemoryCache.<br />
    /// </summary>
    public void TryAddDistributedCache(ILogger logger)
    {
        var hasDistributedCacheSection = Configuration.HasConfigSection(Constants.ConfigSections.DistributedCache);
        if (Environment.IsProduction() && hasDistributedCacheSection)
        {
            this.AddDistributedCache();
            LogMessages.FeatureConfigured(logger, "DistributedCache", Constants.ConfigSections.DistributedCache);
        }
        else
        {
            if (!hasDistributedCacheSection)
            {
                LogMessages.DistributedCacheMissing(logger, Constants.ConfigSections.DistributedCache);
            }

            Services.AddDistributedMemoryCache();
            LogMessages.FeatureAdded(logger, "DistributedMemoryCache");
        }
    }


    /// <summary>
    /// Adds FeatureManagement services from config section 'Features' to the application, if the section exists.
    /// </summary>
    public void TryAddFeatureManagement(ILogger logger)
    {
        // Feature Management
        if (Configuration.HasConfigSection(Constants.ConfigSections.FeatureManagement))
        {
            if (Configuration[Constants.ConfigSections.FeatureManagement]?.StartsWith("http", StringComparison.OrdinalIgnoreCase) == true)
            {
                this.AddRemoteFeatureManagement(httpClient =>
                {
                    httpClient.BaseAddress = new Uri(Configuration[Constants.ConfigSections.FeatureManagement]!);
                });
                LogMessages.FeatureConfigured(logger, "RemoteFeatureManagement", Constants.ConfigSections.FeatureManagement);
            }
            else
            {
                this.AddFeatureManagement();
                LogMessages.FeatureConfigured(logger, "FeatureManagement", Constants.ConfigSections.FeatureManagement);
            }
        }
        else
        {
            LogMessages.MissingOptionalConfig(logger, "FeatureManagement", Constants.ConfigSections.FeatureManagement);
        }

    }

    /// <summary>
    /// Adds OpenTelemetry services from config section 'OpenTelemetry' to the application, if the section exists.
    /// </summary>
    public void TryAddOpenTelemetry(ILogger logger)
    {
        if (Configuration.HasConfigSection(Constants.ConfigSections.OpenTelemetry))
        {
            this.AddOpenTelemetry();
            LogMessages.FeatureConfigured(logger, "OpenTelemetry", Constants.ConfigSections.OpenTelemetry);
        }
        else
        {
            LogMessages.MissingOptionalConfig(logger, "OpenTelemetry", Constants.ConfigSections.OpenTelemetry);
        }
    }
}
