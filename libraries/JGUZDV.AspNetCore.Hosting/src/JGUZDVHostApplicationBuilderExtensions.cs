﻿using System.Text.Json.Serialization;

using JGUZDV.AspNetCore.Hosting.Extensions;
using JGUZDV.Extensions.Json;
using JGUZDV.WebApiHost.FeatureManagement;

using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http.Json;
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
    /// Adds JGUZDVLogging to the WebApplicationBuilder.
    /// </summary>
    public static JGUZDVHostApplicationBuilder AddLogging(
        this JGUZDVHostApplicationBuilder appBuilder,
        Action<ILoggingBuilder>? configure = null)
    {
        appBuilder.Builder.UseJGUZDVLogging();
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


    /// <summary>
    /// Adds Telemetry to the WebApplicationBuilder.
    /// </summary>
    public static JGUZDVHostApplicationBuilder AddOpenTelemetry(
        this JGUZDVHostApplicationBuilder appBuilder,
        string configSection = Constants.ConfigSections.Telemetry)
    {
        //appBuilder.Configuration.ValidateConfigSectionExists(configSection);
        //appBuilder.Services.AddApplicationInsightsTelemetry(
        //    appBuilder.Configuration.GetSection(configSection).Get<string>()!);

        appBuilder.HasOpenTelemetry = true;
        return appBuilder;
    }


    /// <summary>
    /// Adds Health checks to the WebApplicationBuilder.
    /// </summary>
    public static JGUZDVHostApplicationBuilder AddHealthChecks(
        this JGUZDVHostApplicationBuilder appBuilder)
    {
        appBuilder.Services.AddHealthChecks();
        
        appBuilder.HasHealthChecks = true;
        return appBuilder;
    }


    /// <summary>
    /// Adds OpenApi to the WebApplicationBuilder.
    /// </summary>
    public static JGUZDVHostApplicationBuilder AddOpenApi(
        this JGUZDVHostApplicationBuilder appBuilder)
    {
        appBuilder.Services.AddOpenApi();

        appBuilder.HasOpenApi = true;
        return appBuilder;
    }


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

    #region Frontend Frameworks
    /// <summary>
    /// Adds MVC to the WebApplicationBuilder.
    /// This will also configure the JsonOptions MVC controllers.
    /// </summary>
    public static JGUZDVHostApplicationBuilder AddAspNetCoreMvc(
        this JGUZDVHostApplicationBuilder appBuilder,
        bool enableViewSupport,
        Action<IMvcBuilder>? configure = null)
    {
        IMvcBuilder mvcBuilder;
        if (enableViewSupport)
        {
            appBuilder.Services.AddAntiforgery();
            mvcBuilder = appBuilder.Services.AddControllersWithViews();

            appBuilder.HasFrontend = true;
        }
        else
        {
            mvcBuilder = appBuilder.Services.AddControllers();
        }
        
        mvcBuilder.AddJsonOptions(opt => opt.JsonSerializerOptions.SetJGUZDVDefaults());
        configure?.Invoke(mvcBuilder);

        appBuilder.HasMVC = true;
        return appBuilder;
    }


    /// <summary>
    /// Adds RazorPages to the WebApplicationBuilder.
    /// </summary>
    public static JGUZDVHostApplicationBuilder AddRazorPages(
        this JGUZDVHostApplicationBuilder appBuilder,
        Action<IMvcBuilder>? configure = null)
    {
        appBuilder.Services.AddAntiforgery();

        var builder = appBuilder.Services.AddRazorPages();
        configure?.Invoke(builder);

        appBuilder.HasRazorPages = true;
        appBuilder.HasFrontend = true;
        return appBuilder;
    }


    /// <summary>
    /// Adds Blazor to the WebApplicationBuilder.
    /// </summary>
    public static JGUZDVHostApplicationBuilder AddBlazor(
        this JGUZDVHostApplicationBuilder appBuilder,
        bool enableInteractiveComponents = false,
        Action<IRazorComponentsBuilder>? configure = null,
        Action<CircuitOptions>? configureCircuit = null)
    {
        // Add services to the container.
        var builder = appBuilder.Services.AddRazorComponents();
        if (enableInteractiveComponents)
        {
            builder.AddInteractiveServerComponents(configureCircuit);
            appBuilder.HasInteractiveServerComponents = true;
        }

        configure?.Invoke(builder);

        appBuilder.HasRazorComponents = true;
        return appBuilder;
    }


    /// <summary>
    /// Sets JsonOptions for minimal API
    /// </summary>
    public static void ApplyJsonOptions(this JsonOptions opt)
    {
        opt.SerializerOptions.SetJGUZDVDefaults();
    }
    #endregion


    #region JwtBearer Authentication
    /// <summary>
    /// Adds JwtBearer Authentication to the WebApplicationBuilder and adds an authorization check
    /// that ensures one or multiple scopes to be present in the token.
    /// If Authentiation:JwtBearer is not present an error will be thrown
    /// </summary>
    public static JGUZDVHostApplicationBuilder AddDefaultJwtBearerAuthentication(
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
                opt.ApplyDefaultBearerAuthenticationConfig(bearerConfigSection, logger);
                configureJwtBearer?.Invoke(opt);
            });

        authenticationBuilderAction?.Invoke(authBuilder);

        appBuilder.Services.AddAuthorization(opt =>
        {
            ApplyDefaultBearerAuthorizationConfig(opt, bearerConfigSection, logger);
            configureAuthorization?.Invoke(opt);
        });

        appBuilder.HasAuthentication = true;
        return appBuilder;
    }


    /// <summary>
    /// Applies the default configuration for Authorization for JwtBearer.<br />
    /// This is reading AllowedScopes from the config and setting the DefaultPolicy to "DefaultWithScopeCheck".
    /// </summary>
    public static void ApplyDefaultBearerAuthorizationConfig(this AuthorizationOptions opt, IConfigurationSection bearerConfigSection, ILogger logger)
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
    }

    /// <summary>
    /// Applies the default configuration for JwtBearerOptions.<br />
    /// This is binding the configSection to the options, setting the AudienceValidation via "ValidAudiences" and disabling the mapping of claims.
    /// </summary>
    public static void ApplyDefaultBearerAuthenticationConfig(
        this JwtBearerOptions opt,
        IConfigurationSection configSection,
        ILogger logger)
    {
        configSection.Bind(opt);

        var validAudiences = configSection.GetSection("ValidAudiences").Get<ICollection<string>?>() ?? [];
        var validateAudience = validAudiences?.Count > 0;

        opt.TokenValidationParameters.ValidateAudience = validateAudience;
        opt.TokenValidationParameters.ValidAudiences = validAudiences;

        if (opt.TokenValidationParameters.ValidAudiences?.Any() != true)
            LogMessages.NoValidAudiences(logger);

        opt.MapInboundClaims = false;
    }
    #endregion
    
    #region OpenIdConnect Authentication

    #endregion
}