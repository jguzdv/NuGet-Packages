using System.IdentityModel.Tokens.Jwt;
using System.Text.Json.Serialization;

using JGUZDV.Blazor.WasmServerHost.Extensions;
using JGUZDV.Extensions.Json;
using JGUZDV.WebApiHost.Extensions;
using JGUZDV.WebApiHost.FeatureManagement;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
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
    /// <para>Configures services of the WebApplicationBuilder:</para>
    /// <para>- Adds JGUZDVLogging</para>
    /// <para>- Adds ApiExplorer and Swagger</para>
    /// <para>- Adds ProblemDetails</para>
    /// <para>- Adds MVC controllers and sets default JsonOptions (minimal API as well as MVC)</para>
    /// <para>- Adds RequestLocalization ("de", "en" as default)</para>
    /// <para>- Adds Distributed Cache</para>
    /// <para>- Adds Data Protection</para>
    /// <para>- Adds Authentication and Authorization</para>
    /// <para>- Adds Feature Management</para>
    /// <para>- Adds Telemetry</para>
    /// <para>- Adds HealthChecks</para>
    /// </summary>
    public static WebApplicationBuilder ConfigureWebApiHostServices(
        this WebApplicationBuilder builder,
        Action<AuthenticationBuilder>? authenticationBuilderAction = null,
        Action<JwtBearerOptions>? jwtBearerOptionsAction = null,
        Action<AuthorizationOptions>? authorizationOptionsAction = null,
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


}
