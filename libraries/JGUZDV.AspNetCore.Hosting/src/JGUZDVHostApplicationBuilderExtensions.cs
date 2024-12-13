using JGUZDV.AspNetCore.Hosting.Extensions;
using JGUZDV.WebApiHost.FeatureManagement;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.FeatureManagement;

namespace JGUZDV.AspNetCore.Hosting;

/// <summary>
/// Collection of extensions for WebApplicationBuilder.
/// </summary>
public static class JGUZDVHostApplicationBuilderExtensions
{
    /// <summary>
    /// Adds JGUZDV Feature Management to the WebApplicationBuilder.
    /// It expects the config section to be named "Features".
    /// If the configuration section is not found, it will throw an exception.
    /// </summary>
    public static JGUZDVHostApplicationBuilder AddFeatureManagement(
        this JGUZDVHostApplicationBuilder appBuilder,
        Action<IFeatureManagementBuilder>? configure = null,
        string configSection = Constants.ConfigSections.FeatureManagement)
    {
        appBuilder.Configuration.ValidateConfigSectionExists(configSection);

        var builder = appBuilder.Services.AddScopedFeatureManagement(
            appBuilder.Configuration.GetSection(configSection)
            )
            .AddFeatureFilter<ClaimRequirementFeatureFilter>();

        configure?.Invoke(builder);

        appBuilder.HasFeatureManagement = true;
        return appBuilder;
    }


    /// <summary>
    /// Adds JGUZDV DataProtection to the WebApplicationBuilder.
    /// It expects the configuration section to be named "JGUZDV:DataProtection".
    /// If the configuration section is not found, it will throw an exception.
    /// </summary>
    public static JGUZDVHostApplicationBuilder AddDataProtection(
        this JGUZDVHostApplicationBuilder appBuilder,
        Action<IDataProtectionBuilder>? configure = null,
        string configSection = Constants.ConfigSections.DataProtection)
    {
        appBuilder.Configuration.ValidateConfigSectionExists(configSection);
        
        var builder = appBuilder.Builder.AddJGUZDVDataProtection(configSection);
        configure?.Invoke(builder);

        appBuilder.HasDataProtection = true;
        return appBuilder;
    }


    /// <summary>
    /// Adds DistributedSqlCache to the WebApplicationBuilder.
    /// It expects the configuration section to be named "DistributedCache".
    /// If the configuration section is not found, it will throw an exception.
    /// </summary>
    public static JGUZDVHostApplicationBuilder AddDistributedCache(
        this JGUZDVHostApplicationBuilder appBuilder,
        string configSection = Constants.ConfigSections.DistributedCache)
    {
        appBuilder.Configuration.ValidateConfigSectionExists(configSection);

        appBuilder.Services.AddDistributedSqlServerCache(opt => 
            appBuilder.Configuration.GetSection(configSection).Bind(opt));

        appBuilder.HasDistributedCache = true;
        return appBuilder;
    }


    /// <summary>
    /// Adds RequestLocalization to the WebApplicationBuilder.
    /// It tries to load the supported cultures from the configuration section "RequestLocalization:Cultures".
    /// If the config value does not exists, it'll default to ["de-DE", "en-US"].
    /// The current culture will be added as Culture-Content to the response header.
    /// </summary>
    public static JGUZDVHostApplicationBuilder AddLocalization(
        this JGUZDVHostApplicationBuilder appBuilder,
        Action<LocalizationOptions>? configureLocalization = null,
        Action<RequestLocalizationOptions>? configureRequestLocalization = null,
        string configSection = Constants.ConfigSections.RequestLocalization,
        ILogger? logger = null)
    {
        logger ??= NullLogger.Instance;

        string[] locales = ["de-DE", "en-US"];
        if (appBuilder.Configuration.HasConfigSection(configSection))
        {
            locales = appBuilder.Configuration.GetSection($"{configSection}:Cultures").Get<string[]>()!;
        }
        else
        {
            // TODO: Add missing optional logging section
        }

        appBuilder.Services.AddLocalization(opt =>
        {
            configureLocalization?.Invoke(opt);
        });
        appBuilder.Services.AddRequestLocalization(opt =>
        {
            opt
                .AddSupportedCultures(locales)
                .AddSupportedUICultures(locales)
                .SetDefaultCulture(locales[0]);

            opt.ApplyCurrentCultureToResponseHeaders = true;

            configureRequestLocalization?.Invoke(opt);
        });

        appBuilder.HasRequestLocalization = true;
        return appBuilder;
    }


    public static JGUZDVHostApplicationBuilder AddTelemetry(
        this JGUZDVHostApplicationBuilder appBuilder,
        string configSection = Constants.ConfigSections.Telemetry)
    {
        //appBuilder.Configuration.ValidateConfigSectionExists(configSection);
        //appBuilder.Services.AddApplicationInsightsTelemetry(
        //    appBuilder.Configuration.GetSection(configSection).Get<string>()!);

        appBuilder.HasTelemetry = true;
        return appBuilder;
    }


    public static JGUZDVHostApplicationBuilder AddOpenApi(
        this JGUZDVHostApplicationBuilder appBuilder)
    {
        appBuilder.Builder.AddOpenApi();

        appBuilder.HasOpenApi = true;
        return appBuilder;
    }


    /// <summary>
    /// Adds JwtBearer Authentication to the WebApplicationBuilder and adds an authorization check
    /// that ensures one or multiple scopes to be present in the token.
    /// If Authentiation:JwtBearer is not present an error will be thrown
    /// </summary>
    public static JGUZDVHostApplicationBuilder AddJwtBearerAuthentication(
        this JGUZDVHostApplicationBuilder appBuilder,
        Action<AuthenticationBuilder>? authenticationBuilderAction = null,
        Action<AuthenticationOptions>? configureaAuthentication = null,
        Action<JwtBearerOptions>? configureJwtBearer = null,
        Action<AuthorizationOptions>? configureAuthorization = null,
        string configSection = Constants.ConfigSections.JwtBearerAuthentication,
        ILogger? logger = null)
    {
        appBuilder.Configuration.ValidateConfigSectionExists(configSection);

        logger ??= NullLogger.Instance;

        var bearerConfigSection = appBuilder.Configuration.GetSection(configSection);

        var authBuilder = appBuilder.Services
            .AddAuthentication(opt => {
                opt.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;

                configureaAuthentication?.Invoke(opt);
            })
            .AddJwtBearer(opt =>
            {
                bearerConfigSection.Bind(opt);

                var validAudiences = bearerConfigSection.GetSection("ValidAudiences").Get<ICollection<string>?>() ?? [];
                var validateAudience = validAudiences?.Count > 0;

                opt.TokenValidationParameters.ValidateAudience = validateAudience;
                opt.TokenValidationParameters.ValidAudiences = validAudiences;

                if (opt.TokenValidationParameters.ValidAudiences?.Any() != true)
                    LogMessages.NoValidAudiences(logger);

                opt.MapInboundClaims = false;

                configureJwtBearer?.Invoke(opt);
            });

        authenticationBuilderAction?.Invoke(authBuilder);

        appBuilder.Services.AddAuthorization(opt =>
        {
            var scopes = bearerConfigSection.GetSection("AllowedScopes").Get<string[]>() ?? [];

            if (scopes.Length == 0)
            {
                LogMessages.NoRequiredScopes(logger);
            }
            else
            {
                var scopeClaimType = bearerConfigSection["ScopeType"] ?? "scope";

                opt.AddPolicy("DefaultWithScopeCheck", p =>
                {
                    p.RequireAuthenticatedUser();
                    p.RequireAnyScope(scopes);
                });
                opt.DefaultPolicy = opt.GetPolicy("DefaultWithScopeCheck")!;
            }

            configureAuthorization?.Invoke(opt);
        });

        appBuilder.HasAuthentication = true;
        return appBuilder;
    }
}
