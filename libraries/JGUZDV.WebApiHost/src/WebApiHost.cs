using JGUZDV.Blazor.WasmServerHost.Extensions;
using JGUZDV.WebApiHost.FeatureManagement;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.FeatureManagement;

namespace JGUZDV.WebApiHost;

public static partial class WebApiHost
{
    public static class ConfigSections
    {
        public const string Authentication = "Authentication";
        public const string DataProtection = AspNetCore.DataProtection.Constants.DefaultSectionName;
        public const string DistributedCache = "DistributedCache";
        public const string FeatureManagement = "FeatureManagement";
        public const string Telemetry = "ApplicationInsights";
    }


    public static WebApplicationBuilder ConfigureWebApiHostServices(
        this WebApplicationBuilder builder,
        bool useInteractiveWebAssembly
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
            var logger = loggerFactory?.CreateLogger(nameof(WebApiHost)) ?? NullLogger.Instance;

            // Enable ApiExplorer
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();


            // Enable MVC controllers
            services.AddControllers();
            services.AddProblemDetails();


            // Add Localization for DE, EN and RequestLocaltization
            services.AddLocalization();
            var locales = config
                .GetSection("RequestLocalization:Cultures")
                .Get<string[]>() ?? ["de", "en"];

            services.AddRequestLocalization(opt => opt
                .AddSupportedCultures(locales)
                .AddSupportedUICultures(locales)
                .SetDefaultCulture(locales[0])
            );


            // Add distributed cache
            if (environment.IsProduction() && config.HasConfigSection(ConfigSections.DistributedCache))
            {
                services.AddDistributedSqlServerCache(opt => config.GetSection(ConfigSections.DistributedCache).Bind(opt));
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
                var configSection = config.GetSection($"{ConfigSections.Authentication}:JwtBearer");

                var authBuilder = services
                    .AddAuthentication(opt => opt.DefaultScheme = JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(opt =>
                    {
                        var validAudiences = configSection.GetSection("ValidAudiences").Get<ICollection<string>?>() ?? [];
                        var validateAudience = validAudiences?.Count > 0;

                        opt.TokenValidationParameters.ValidateAudience = validateAudience;
                        opt.TokenValidationParameters.ValidAudiences = validAudiences;

                        config.GetSection(ConfigSections.Authentication).Bind(opt);

                        if (opt.TokenValidationParameters.ValidAudiences?.Any() != true)
                            Log.NoValidAudiences(logger);
                    });

                services.AddAuthorization(opt =>
                {
                    var scopes = configSection.GetSection("RequiredScopes").Get<ICollection<string>?>() ?? [];
                    if (scopes.Count == 0)
                    {
                        Log.NoRequiredScopes(logger);
                    }
                    else
                    {
                        var scopeClaimType = configSection["ScopeType"] ?? "scope";

                        opt.AddPolicy("ScopedDefault", p =>
                        {
                            p.RequireAuthenticatedUser();
                            p.RequireClaim("scope", scopes);

                        });
                        opt.DefaultPolicy = opt.GetPolicy("ScopedDefault")!;
                    }
                });

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


            // Feature Management
            if (config.HasConfigSection(ConfigSections.FeatureManagement))
            {
                services.AddScopedFeatureManagement(config.GetSection(ConfigSections.FeatureManagement))
                    .AddFeatureFilter<ClaimRequirementFeatureFilter>();
            }
            else
            {
                Log.MissingConfig(logger, ConfigSections.FeatureManagement);
            }


            // Telemetry
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



    public static WebApplication ConfigureWebApiHost(this WebApplication app)
    {
        var conf = app.Configuration;
        var env = app.Environment;

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }


        app.UseHttpsRedirection();

        app.UseRequestLocalization();
        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
        
        app.MapHealthChecks("/health");
        app.MapGet("/docs", ctx =>
        {
            ctx.Response.Redirect("/swagger");
            return Task.CompletedTask;
        });

        app.MapSwagger();
        app.UseSwaggerUI();

        return app;
    }


    private partial class Log
    {
        [LoggerMessage(LogLevel.Information, "Could not find config {configSection}. The corresponding feature will not be added to services or pipeline.")]
        public static partial void MissingConfig(ILogger logger, string configSection);

        [LoggerMessage(LogLevel.Information, "Could not find config Authentication:JwtBearer:ValidAudiences, audiences will not be considered.")]
        public static partial void NoValidAudiences(ILogger logger);

        [LoggerMessage(LogLevel.Warning, "Could not find config Authentication:JwtBearer:RequiredScopes, consider addings required scopes to validate the token is meant for us.")]
        public static partial void NoRequiredScopes(ILogger logger);
    }
}
