using JGUZDV.Blazor.WasmHost.Extensions;
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

    public static WebApplicationBuilder ConfigureWasmHostServices(this WebApplicationBuilder builder)
    {
        builder.Services.ConfigureWasmHostServices(builder.Configuration, builder.Environment);
        return builder;
    }


    public static IServiceCollection ConfigureWasmHostServices(
        this IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        var sp = services.BuildServiceProvider();
        var loggerFactory = sp.GetService<ILoggerFactory>();

        try
        {
            var logger = loggerFactory?.CreateLogger(nameof(BlazorWasmHost)) ?? NullLogger.Instance;

            // Enable MVC controllers and Razor pages
            services.AddControllers();
            services.AddRazorPages();

            // Add Localization for DE, EN and RequestLocaltization
            services.AddLocalization();
            var locales = configuration
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
            if (configuration.HasConfigSection(ConfigSections.ReverseProxy))
            {
                services.AddSimpleReverseProxy(ConfigSections.ReverseProxy);
            }
            else
            {
                Log.MissingConfig(logger, ConfigSections.ReverseProxy);
            }


            // Add distributed cache
            if (environment.IsProduction() && configuration.HasConfigSection(ConfigSections.DistributedCache))
            {
                services.AddDistributedSqlServerCache(opt =>
                {
                    configuration.GetSection(ConfigSections.DistributedCache).Bind(opt);
                });
            }
            else
            {
                services.AddDistributedMemoryCache();
            }

            if (!configuration.HasConfigSection(ConfigSections.DistributedCache))
                Log.MissingConfig(logger, ConfigSections.DistributedCache);


            // Add data protection
            if (environment.IsProduction() && configuration.HasConfigSection(ConfigSections.DataProtection))
            {
                services.AddJGUZDVDataProtection(configuration, environment);
            }
            else
            {
                services.AddDataProtection();
            }

            if (!configuration.HasConfigSection(ConfigSections.DataProtection))
                Log.MissingConfig(logger, ConfigSections.DataProtection);


            // Add authentication and authorization
            if (configuration.HasConfigSection(ConfigSections.Authentication))
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

                        var oidcConfig = configuration.GetSection(ConfigSections.Authentication + ":OpenIdConnect");
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

                        var cookieConfig = configuration.GetSection(ConfigSections.Authentication + ":Cookie");
                        cookieConfig.Bind(opt);
                    })
                    .AddCookieDistributedTicketStore();

                services.AddAuthorization();

                services.AddScoped<AuthenticationStateProvider, ServerAuthenticationStateProvider>();
                services.AddCascadingAuthenticationState();
            }
            else
            {
                Log.MissingConfig(logger, ConfigSections.Authentication);
            }



            if (configuration.HasConfigSection(ConfigSections.Telemetry))
            {
                // TODO: Add default calls for Telemetry and Healthchecks
                services.AddHealthChecks();
            }
            else
            {
                Log.MissingConfig(logger, ConfigSections.Telemetry);
                services.AddHealthChecks();
            }

            return services;
        }
        finally
        {
            loggerFactory?.Dispose();
        }
    }



    public static WebApplication ConfigureWasmHost(this WebApplication app)
    {
        ConfigureWasmHost(app, app.Configuration, app.Environment);
        return app;
    }


    public static IApplicationBuilder ConfigureWasmHost(
        this IApplicationBuilder app,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        if (environment.IsDevelopment())
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

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapRazorPages();
            endpoints.MapFallbackToPage("/Index");
            endpoints.MapZdvHealthEndpoint();

            if (configuration.HasConfigSection(ConfigSections.ReverseProxy))
            {
                endpoints.MapSimpleReverseProxy();
            }
        });

        return app;
    }


    private partial class Log
    {
        [LoggerMessage(LogLevel.Information, "Could not find config {configSection}. The corresponding feature will not be added to Services or Pipeline")]
        public static partial void MissingConfig(ILogger logger, string configSection);
    }
}
