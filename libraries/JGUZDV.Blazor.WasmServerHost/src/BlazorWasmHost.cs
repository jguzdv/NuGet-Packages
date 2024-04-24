using JGUZDV.Blazor.WasmServerHost.Extensions;
using JGUZDV.YARP.SimpleReverseProxy;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.Extensions.Logging.Abstractions;

namespace JGUZDV.Blazor.WasmHost;

public static partial class BlazorWasmHost
{
    public static class ConfigSections
    {
        public const string Authentication = "Authentication";
        public const string DataProtection = AspNetCore.DataProtection.Constants.DefaultSectionName;
        public const string DistributedCache = "DistributedCache";
        public const string ReverseProxy = "ReverseProxy";
        public const string Telemetry = "ApplicationInsights";
    }


    public static WebApplicationBuilder ConfigureWasmHostServices(
        this WebApplicationBuilder builder,
        bool useInteractiveWebAssembly
        )
    {
        var services = builder.Services;
        var config = builder.Configuration;
        var env = builder.Environment;

        builder.UseJGUZDVLogging();

        var sp = services.BuildServiceProvider();
        var loggerFactory = sp.GetService<ILoggerFactory>();

        try
        {
            var logger = loggerFactory?.CreateLogger(nameof(BlazorWasmHost)) ?? NullLogger.Instance;

            // Enable MVC controllers and Razor pages
            services.AddControllers();
            services.AddRazorPages();

            // Enable InteractiveWebAssembly
            if (useInteractiveWebAssembly)
            {
                builder.Services.AddRazorComponents()
                    .AddInteractiveWebAssemblyComponents();
            }

            // Add Localization for DE, EN and RequestLocaltization
            services.AddLocalization();
            var locales = config
                .GetSection("RequestLocalization:Cultures")
                .Get<string[]>() ?? ["de", "en"];

            services.AddRequestLocalization(options =>
            {
                options
                    .AddSupportedCultures(locales)
                    .AddSupportedUICultures(locales)
                    .SetDefaultCulture(locales[0]);
            });


            // Add reverse proxy
            if (config.HasConfigSection(ConfigSections.ReverseProxy))
            {
                services.AddSimpleReverseProxy(ConfigSections.ReverseProxy);
            }
            else
            {
                Log.MissingConfig(logger, ConfigSections.ReverseProxy);
            }


            // Add distributed cache
            if (env.IsProduction() && config.HasConfigSection(ConfigSections.DistributedCache))
            {
                services.AddDistributedSqlServerCache(opt =>
                {
                    config.GetSection(ConfigSections.DistributedCache).Bind(opt);
                });
            }
            else
            {
                services.AddDistributedMemoryCache();
            }

            if (!config.HasConfigSection(ConfigSections.DistributedCache))
                Log.MissingConfig(logger, ConfigSections.DistributedCache);


            // Add data protection
            if (env.IsProduction() && config.HasConfigSection(ConfigSections.DataProtection))
            {
                services.AddJGUZDVDataProtection(config, env);
            }
            else
            {
                services.AddDataProtection();
            }

            if (!config.HasConfigSection(ConfigSections.DataProtection))
                Log.MissingConfig(logger, ConfigSections.DataProtection);


            // Add authentication and authorization
            if (config.HasConfigSection(ConfigSections.Authentication))
            {
                var authBuilder = services.AddAuthentication(opt =>
                {
                    opt.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    opt.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                })
                    .AddOpenIdConnect(opt =>
                    {
                        opt.UseTokenLifetime = true;
                        opt.SaveTokens = true;
                        opt.GetClaimsFromUserInfoEndpoint = true;

                        var oidcConfig = config.GetSection(ConfigSections.Authentication + ":OpenIdConnect");
                        oidcConfig.Bind(opt);

                        opt.Scope.Clear();
                        var scopes = oidcConfig.GetSection(nameof(opt.Scope)).GetChildren().Select(element => element.Value).OfType<string>();

                        foreach (var scope in scopes)
                            opt.Scope.Add(scope);
                    })
                    .AddCookie(opt =>
                    {
                        opt.Cookie.Name = env.ApplicationName;
                        opt.SlidingExpiration = false;

                        var cookieConfig = config.GetSection(ConfigSections.Authentication + ":Cookie");
                        cookieConfig.Bind(opt);
                    })
                    .AddCookieDistributedTicketStore();

                services.AddAuthorization();

                if (useInteractiveWebAssembly)
                {
                    services.AddScoped<AuthenticationStateProvider, ServerAuthenticationStateProvider>();
                    services.AddCascadingAuthenticationState();
                }
            }
            else
            {
                Log.MissingConfig(logger, ConfigSections.Authentication);
            }



            if (config.HasConfigSection(ConfigSections.Telemetry))
            {
                // TODO: Add default calls for Telemetry and Healthchecks
                services.AddHealthChecks();
            }
            else
            {
                Log.MissingConfig(logger, ConfigSections.Telemetry);
                services.AddHealthChecks();
            }

            return builder;
        }
        finally
        {
            loggerFactory?.Dispose();
        }
    }


    /// <summary>
    /// Add the following middleware to the pipeline:
    /// - DeveloperExceptionPage (Development only)
    /// - WebAssemblyDebugging (Development only)
    /// - HttpsRedirection
    /// - StaticFiles
    /// - RequestLocalization
    /// - Routing
    /// - Authentication
    /// - Authorization
    /// - Antiforgery
    /// - RazorComponents
    /// - Controllers
    /// - HealthChecks
    /// - ReverseProxy
    /// </summary>
    public static WebApplication ConfigureWasmHost<TBlazorApp>(this WebApplication app,
        params System.Reflection.Assembly[] assemblies)
    {
        var conf = app.Configuration;
        var env = app.Environment;

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseWebAssemblyDebugging();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }


        app.UseHttpsRedirection();

        app.UseStaticFiles();

        app.UseRequestLocalization();
        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseAntiforgery();

        app.MapRazorComponents<TBlazorApp>()
            .AddInteractiveWebAssemblyRenderMode()
            .AddAdditionalAssemblies(assemblies);

        app.MapControllers();
        
        app.MapHealthChecks("/health");

        if (conf.HasConfigSection(ConfigSections.ReverseProxy))
        {
            app.MapSimpleReverseProxy();
        }


        return app;
    }


    public static WebApplication ConfigureWasmHost(
        this WebApplication app)
    {
        var conf = app.Configuration;
        var env = app.Environment;

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseWebAssemblyDebugging();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }


        app.UseHttpsRedirection();

        app.UseBlazorFrameworkFiles();
        app.UseStaticFiles();
        
        app.UseRequestLocalization();
        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
        app.MapRazorPages();
        app.MapFallbackToPage("/Index");

        app.MapHealthChecks("/health");

        if (conf.HasConfigSection(ConfigSections.ReverseProxy))
        {
            app.MapSimpleReverseProxy();
        }


        return app;
    }


    private partial class Log
    {
        [LoggerMessage(LogLevel.Information, "Could not find config {configSection}. The corresponding feature will not be added to Services or Pipeline")]
        public static partial void MissingConfig(ILogger logger, string configSection);
    }
}
