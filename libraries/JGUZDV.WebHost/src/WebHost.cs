using JGUZDV.WebHost.Extensions;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;


namespace JGUZDV.WebHost;

/// <summary>
/// The WebHost class is a static class that provides methods for configuring 
/// and setting up a web host in an ASP.NET Core application. 
/// It contains two main methods: ConfigureWebHostServices and ConfigureWebHost.
/// </summary>
public static partial class WebHost
{
    internal static class ConfigSections
    {
        public const string Authentication = "Authentication";
        public const string DataProtection = AspNetCore.DataProtection.Constants.DefaultSectionName;
        public const string DistributedCache = "DistributedCache";
        public const string Telemetry = "ApplicationInsights";
    }
    /// <summary>
    /// Configures services of the WebApplicationBuilder:
    /// - Adds authentication and authorization
    /// - Adds MVC controllers and Razor pages
    /// - Adds distributed cache
    /// - Adds data protection
    /// - Adds Localization for DE, EN and RequestLocalization
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static WebApplicationBuilder ConfigureWebHostServices(
               this WebApplicationBuilder builder
               )
    {
        var services = builder.Services;
        var config = builder.Configuration;
        var environment = builder.Environment;

        builder.UseJGUZDVLogging();

        var sp = services.BuildServiceProvider();
        var loggerFactory = sp.GetService<ILoggerFactory>();

        try
        {
            var logger = loggerFactory?.CreateLogger(nameof(WebHost)) ?? NullLogger.Instance;

            // Enable MVC controllers and Razor pages
            services.AddControllers();
            services.AddRazorPages();
            services.AddControllersWithViews();


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

            // Add distributed cache
            if (environment.IsProduction() && config.HasConfigSection(ConfigSections.DistributedCache))
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
            if (environment.IsProduction() && config.HasConfigSection(ConfigSections.DataProtection))
            {
                services.AddJGUZDVDataProtection(config, environment);
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
                        opt.Cookie.Name = environment.ApplicationName;
                        opt.SlidingExpiration = false;

                        var cookieConfig = config.GetSection(ConfigSections.Authentication + ":Cookie");
                        cookieConfig.Bind(opt);
                    })
                    .AddCookieDistributedTicketStore();

                services.AddAuthorization();

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

        }
        finally
        {
            loggerFactory?.Dispose();
        }

        return builder;
    }

    /// <summary>
    /// Configures the web application pipeline:
    /// - Adds DeveloperExceptionPage (in Development)
    /// - Adds Swagger
    /// - Adds HTTPS redirection
    /// - Adds Using static files
    /// - Adds Routing
    /// - Adds RequestLocalization
    /// - Adds Authentication
    /// - Adds Authorization
    /// - Adds Razor pages
    /// - Adds Controllers
    /// - Adds Healthchecks
    /// - Adds Redirect from /docs to Swagger
    /// </summary>
    public static WebApplication ConfigureWebHost(this WebApplication app)
    {
        var conf = app.Configuration;
        var env = app.Environment;

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.MapSwagger();
        app.UseSwaggerUI();

        app.UseHttpsRedirection();
        app.UseStaticFiles();


        app.UseRouting();
        app.UseRequestLocalization();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapRazorPages();
        app.MapControllers();

        app.MapHealthChecks("/health");
        app.MapGet("/docs", ctx =>
        {
            ctx.Response.Redirect("/swagger");
            return Task.CompletedTask;
        });


        return app;
    }


    private partial class Log
    {
        [LoggerMessage(LogLevel.Information, "Could not find config {configSection}. The corresponding feature will not be added to Services or Pipeline")]
        public static partial void MissingConfig(ILogger logger, string configSection);
    }
}
