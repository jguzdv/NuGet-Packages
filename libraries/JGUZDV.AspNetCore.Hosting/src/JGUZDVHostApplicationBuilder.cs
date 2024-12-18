using JGUZDV.AspNetCore.Hosting.Authentication;
using JGUZDV.AspNetCore.Hosting.Extensions;
using JGUZDV.AspNetCore.Hosting.FeatureManagement;
using JGUZDV.YARP.SimpleReverseProxy;

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
    /// - OpenTelemetry*<br />
    /// - HealthChecks<br />
    /// - ReverseProxy*<br />
    /// - MVC (with view support)<br />
    /// - RazorPages<br />
    /// - Razor WebComponents (without interactivity aka. Blazor Static Server)<br />
    /// </summary>
    public static JGUZDVHostApplicationBuilder CreateWebHost(string[] args, BlazorInteractivityModes interactivityMode = BlazorInteractivityModes.DisableBlazor)
    {
        var builder = Create(args);

        builder.AddLogging();

        using (var sp = builder.Services.BuildServiceProvider())
        using (var loggerFactory = sp.GetRequiredService<ILoggerFactory>())
        {
            var logger = loggerFactory.CreateLogger(nameof(JGUZDVHostApplicationBuilder));
            var missingConfigLogLevel = builder.Environment.IsProduction() ? LogLevel.Information : LogLevel.Warning;

            builder.Services.AddProblemDetails();

            // TODO: Enable swagger UI?
            builder.AddOpenApi();

            builder.AddLocalization(logger: logger);

            var hasDistributedCacheSection = builder.Configuration.HasConfigSection(Constants.ConfigSections.DistributedCache);
            if (builder.Environment.IsProduction() && hasDistributedCacheSection)
            {
                builder.AddDistributedCache();
            }
            else
            {
                if (!hasDistributedCacheSection)
                {
                    LogMessages.MissingConfig(logger, missingConfigLogLevel, Constants.ConfigSections.DistributedCache);
                }

                builder.Services.AddDistributedMemoryCache();
            }

            var hasDataProtectionSection = builder.Configuration.HasConfigSection(Constants.ConfigSections.DataProtection);
            if (builder.Environment.IsProduction() && hasDataProtectionSection)
            {
                builder.AddDataProtection();
            }
            else
            {
                if (!hasDataProtectionSection)
                {
                    LogMessages.MissingConfig(logger, missingConfigLogLevel, Constants.ConfigSections.DataProtection);
                }

                builder.Services.AddDataProtection();
            }

            if (builder.Configuration.HasConfigSection(Constants.ConfigSections.OpenIdConnectAuthentication))
            {
                builder.AddDefaultOpenIdConnectAuthentication(logger: logger);
            }
            else
            {
                LogMessages.MissingConfig(logger, missingConfigLogLevel, Constants.ConfigSections.OpenIdConnectAuthentication);
            }


            if (builder.Configuration.HasConfigSection(Constants.ConfigSections.FeatureManagement))
            {
                builder.AddFeatureManagement();
            }
            else
            {
                LogMessages.MissingConfig(logger, missingConfigLogLevel, Constants.ConfigSections.FeatureManagement);
            }


            if (builder.Configuration.HasConfigSection(Constants.ConfigSections.OpenTelemetry))
            {
                builder.AddOpenTelemetry();
            }
            else
            {
                LogMessages.MissingConfig(logger, missingConfigLogLevel, Constants.ConfigSections.OpenTelemetry);
            }

            builder.AddHealthChecks();

            if (builder.Configuration.HasConfigSection(Constants.ConfigSections.ReverseProxy))
            {
                builder.AddReverseProxy();
            }
            else
            {
                LogMessages.MissingConfig(logger, missingConfigLogLevel, Constants.ConfigSections.ReverseProxy);
            }

            builder.AddAspNetCoreMvc(enableViewSupport: true);
            builder.AddRazorPages();
            if (interactivityMode != BlazorInteractivityModes.DisableBlazor)
            {
                builder.AddBlazor(interactivityMode);
            }
            builder.Services.ConfigureHttpJsonOptions(opt => opt.ApplyJsonOptions());
        }

        return builder;
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
    /// - OpenTelemetry*<br />
    /// - HealthChecks<br />
    /// - ReverseProxy*<br />
    /// - MVC (with view support)<br />
    /// - RazorPages<br />
    /// - Razor WebComponents (without interactivity aka. Blazor Static Server)<br />
    /// </summary>
    public static JGUZDVHostApplicationBuilder CreateStaticWeb(string[] args)
        => CreateWebHost(args, BlazorInteractivityModes.None);


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
    public static JGUZDVHostApplicationBuilder CreateWebApi(string[] args)
    {
        var builder = Create(args);

        builder.AddLogging();

        using (var sp = builder.Services.BuildServiceProvider())
        using (var loggerFactory = sp.GetRequiredService<ILoggerFactory>())
        {
            var logger = loggerFactory.CreateLogger(nameof(JGUZDVHostApplicationBuilder));
            var missingConfigLogLevel = builder.Environment.IsProduction() ? LogLevel.Information : LogLevel.Warning;

            builder.Services.AddProblemDetails();

            // TODO: Enable swagger UI?
            builder.AddOpenApi();
            builder.AddAspNetCoreMvc(enableViewSupport: false);
            builder.Services.ConfigureHttpJsonOptions(opt => opt.ApplyJsonOptions());

            builder.AddLocalization(logger: logger);

            var hasDistributedCacheSection = builder.Configuration.HasConfigSection(Constants.ConfigSections.DistributedCache);
            if (builder.Environment.IsProduction() && hasDistributedCacheSection)
            {
                builder.AddDistributedCache();
            }
            else
            {
                if (!hasDistributedCacheSection)
                {
                    LogMessages.MissingConfig(logger, missingConfigLogLevel, Constants.ConfigSections.DistributedCache);
                }

                builder.Services.AddDistributedMemoryCache();
            }

            var hasDataProtectionSection = builder.Configuration.HasConfigSection(Constants.ConfigSections.DataProtection);
            if (builder.Environment.IsProduction() && hasDataProtectionSection)
            {
                builder.AddDataProtection();
            }
            else
            {
                if (!hasDataProtectionSection)
                {
                    LogMessages.MissingConfig(logger, missingConfigLogLevel, Constants.ConfigSections.DataProtection);
                }

                builder.Services.AddDataProtection();
            }

            if (builder.Configuration.HasConfigSection(Constants.ConfigSections.JwtBearerAuthentication))
            {
                builder.AddDefaultJwtBearerAuthentication(logger: logger);
            }
            else
            {
                LogMessages.MissingConfig(logger, missingConfigLogLevel, Constants.ConfigSections.JwtBearerAuthentication);
            }


            if (builder.Configuration.HasConfigSection(Constants.ConfigSections.FeatureManagement))
            {
                builder.AddFeatureManagement();
            }
            else
            {
                LogMessages.MissingConfig(logger, missingConfigLogLevel, Constants.ConfigSections.FeatureManagement);
            }


            if (builder.Configuration.HasConfigSection(Constants.ConfigSections.OpenTelemetry))
            {
                builder.AddOpenTelemetry();
            }
            else
            {
                LogMessages.MissingConfig(logger, missingConfigLogLevel, Constants.ConfigSections.OpenTelemetry);
            }

            builder.AddHealthChecks();
        }

        return builder;
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

                if (HasRequestLocalization)
                {
                    app.UseRequestLocalizationSerialization();
                }
            }
        }

        return app;
    }


    private WebApplication ConfigureDefaultApp(WebApplication app)
    {
        app.UseStaticFiles();

        if (HasFrontend)
        {
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error", createScopeForErrors: true);
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
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

            // In Blazor WebAssembly, we'll use this to serialize the request localization settings to the client.
            if (HasInteractiveWebAssemblyComponents)
            {
                app.UseMiddleware<RequestLocalizationSerializationMiddleware>();
            }
        }

        if (HasReverseProxy)
        {
            app.MapSimpleReverseProxy();
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

        if (HasAuthentication)
        {
            app.MapAuthentication();
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
