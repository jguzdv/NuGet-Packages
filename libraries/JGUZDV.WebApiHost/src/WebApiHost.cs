using System.IdentityModel.Tokens.Jwt;
using System.Text.Json.Serialization;

using JGUZDV.Blazor.WasmServerHost.Extensions;
using JGUZDV.Extensions.Json;
using JGUZDV.WebApiHost.Extensions;
using JGUZDV.WebApiHost.FeatureManagement;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.FeatureManagement;
using Microsoft.IdentityModel.Logging;

namespace JGUZDV.WebApiHost;

/// <summary>
/// WebApiHost is a helper class to configure a WebApiHost with common services and features.
/// </summary>
public static partial class WebApiHost
{
    /// <summary>
    /// Default Config Sections for WebApiHost
    /// </summary>
    public static class ConfigSections
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public const string Authentication = "Authentication";
        public const string DataProtection = AspNetCore.DataProtection.Constants.DefaultSectionName;
        public const string DistributedCache = "DistributedCache";
        public const string FeatureManagement = "FeatureManagement";
        public const string Telemetry = "ApplicationInsights";
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }

    /// <summary>
    /// Configures services of the WebApplicationBuilder:
    /// - Adds JGUZDVLogging
    /// - Adds ApiExplorer and Swagger
    /// - Adds ProblemDetails
    /// - Adds MVC controllers and sets default JsonOptions (minimal API as well as MVC)
    /// - Adds RequestLocalization ("de", "en" as default)
    /// - Adds Distributed Cache
    /// - Adds Data Protection
    /// - Adds Authentication and Authorization
    /// - Adds Feature Management
    /// - Adds Telemetry
    /// - Adds HealthChecks
    /// </summary>
    public static WebApplicationBuilder ConfigureWebApiHostServices(
        this WebApplicationBuilder builder,
        Action<AuthenticationBuilder>? authenticationBuilderAction = null,
        Action<IDataProtectionBuilder>? dataProtectionBuilderAction = null,
        Action<IFeatureManagementBuilder>? featureManagementBuilderAction = null,
        Action<IMvcBuilder>? mvcBuilderAction = null
        )
    {
        var services = builder.Services;
        var config = builder.Configuration;
        var environment = builder.Environment;

        JwtSecurityTokenHandler.DefaultMapInboundClaims = false;
        IdentityModelEventSource.ShowPII = builder.Configuration.GetValue<bool>("ShowPII", false);

        builder.UseJGUZDVLogging();

        var sp = services.BuildServiceProvider();
        var loggerFactory = sp.GetService<ILoggerFactory>();

        try
        {
            var logger = loggerFactory?.CreateLogger(nameof(WebApiHost)) ?? NullLogger.Instance;

            // Enable ApiExplorer
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            // Enable Problem Details
            services.AddProblemDetails();
            
            // Enable MVC controllers
            var mvcBuilder = services.AddControllers();
            mvcBuilderAction?.Invoke(mvcBuilder);
            mvcBuilder.AddJsonOptions(opt =>
            {
                opt.JsonSerializerOptions.SetJGUZDVDefaults();
                opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

            // Set json options for minimal API
            services.Configure<JsonOptions>(opt =>
            {
                opt.SerializerOptions.SetJGUZDVDefaults();
                opt.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });


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
            IDataProtectionBuilder dataProtectionBuilder = 
                environment.IsProduction() && config.HasConfigSection(ConfigSections.DataProtection)
                    ? services.AddJGUZDVDataProtection(config, environment)
                    : services.AddDataProtection();

            dataProtectionBuilderAction?.Invoke(dataProtectionBuilder);

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

                authenticationBuilderAction?.Invoke(authBuilder);

                services.AddAuthorization(opt =>
                {
                    // RequiredScopes as name was misleading, since it requires one of those scopes to be present,
                    // to not introduce breaking changes, we will keep the name and add AllowedScopes as well
                    var scopes = (configSection.GetSection("RequiredScopes").Get<ICollection<string>?>() ?? [])
                        .Concat(configSection.GetSection("AllowedScopes").Get<ICollection<string>?>() ?? [])
                        .ToList();

                    if (scopes.Count == 0)
                    {
                        Log.NoRequiredScopes(logger);
                    }
                    else
                    {
                        var scopeClaimType = configSection["ScopeType"] ?? "scope";

                        opt.AddPolicy("DefaultWithScopeCheck", p =>
                        {
                            p.RequireAuthenticatedUser();
                            p.RequireAnyScope(scopes);
                        });
                        opt.DefaultPolicy = opt.GetPolicy("DefaultWithScopeCheck")!;
                    }
                });
            }
            else
            {
                Log.MissingConfig(logger, ConfigSections.Authentication);
            }


            // Feature Management
            if (config.HasConfigSection(ConfigSections.FeatureManagement))
            {
                var featureManagementBuilder = services.AddScopedFeatureManagement(
                        config.GetSection(ConfigSections.FeatureManagement)
                    )
                    .AddFeatureFilter<ClaimRequirementFeatureFilter>();

                featureManagementBuilderAction?.Invoke(featureManagementBuilder);
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


    /// <summary>
    /// Configures the web application pipeline:
    /// - Adds DeveloperExceptionPage (in Development)
    /// - Adds Routing
    /// - Adds Authentication and Authorization
    /// - Adds RequestLocalization
    /// - Adds Controllers
    /// - Adds HealthChecks
    /// - Adds Swagger
    /// - Adds SwaggerUI
    /// </summary>
    public static WebApplication ConfigureWebApiHost(this WebApplication app)
    {
        var conf = app.Configuration;
        var env = app.Environment;

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }


        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseRequestLocalization();

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

        [LoggerMessage(LogLevel.Warning, "Could not find config Authentication:JwtBearer:RequiredScopes or Authentication:JwtBearer:AllowedScopes, consider addings required scopes to validate the token is meant for us.")]
        public static partial void NoRequiredScopes(ILogger logger);
    }
}
