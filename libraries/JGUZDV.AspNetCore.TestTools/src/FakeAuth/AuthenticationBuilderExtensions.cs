using JGUZDV.AspNetCore.TestTools.FakeAuth;

using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Provides extension methods for configuring fake authentication in the authentication pipeline for testing and development purposes.
/// </summary>
public static class FakeAuthenticationBuilderExtensions
{
    /// <summary>
    /// Adds the FakeAuthenticationHandler to the authentication pipeline, allowing for the simulation of authentication scenarios without requiring a real authentication provider.
    /// </summary>
    public static AuthenticationBuilder AddFakeAuth(this AuthenticationBuilder builder,
        Action<FakeAuthenticationOptions>? setupAction = null)
    {
        builder.AddScheme<FakeAuthenticationOptions, FakeAuthenticationHandler>(FakeAuthDefaults.AuthenticationScheme, setupAction ?? (_ => { }));

        return builder;
    }

    /// <summary>
    /// Adds the FakeAuthenticationHandler to the authentication pipeline, allowing for the simulation of authentication scenarios without requiring a real authentication provider.
    /// </summary>
    public static AuthenticationBuilder AddFakeAuth(this AuthenticationBuilder builder,
        string? authenticationScheme = null,
        Action<FakeAuthenticationOptions>? setupAction = null)
    {
        builder.AddScheme<FakeAuthenticationOptions, FakeAuthenticationHandler>(authenticationScheme ?? FakeAuthDefaults.AuthenticationScheme, setupAction ?? (_ => { }));

        return builder;
    }
}

/// <summary>
/// Provides extension methods for registering and conditionally enabling fake authentication from configuration.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Use this to configure and use Fake-Auth as defined in configSection.
    /// Always check for Environment.IsDevelopment AND ShouldUseFakeAuth before calling this.
    /// Add a ConfigSection "FakeAuth" containing "Claims" with "Type" and "Value" to modify the contents
    /// </summary>
    public static AuthenticationBuilder AddFakeAuth(this IServiceCollection services, string configSectionPath = "FakeAuth")
    {
        var authOptions = services.AddOptions<FakeAuthenticationOptions>(FakeAuthDefaults.AuthenticationScheme)
            .BindConfiguration(configSectionPath);

        return services.AddAuthentication(opts =>
        {
            opts.DefaultScheme =
                opts.DefaultAuthenticateScheme =
                opts.DefaultChallengeScheme =
                opts.DefaultForbidScheme =
                opts.DefaultSignInScheme =
                opts.DefaultSignOutScheme =
                FakeAuthDefaults.AuthenticationScheme;
        })
            .AddScheme<FakeAuthenticationOptions, FakeAuthenticationHandler>(FakeAuthDefaults.AuthenticationScheme, null);
    }

    /// <summary>
    /// Gets a value indicating whether fake authentication should be used based on the specified configuration section.
    /// </summary>
    public static bool ShouldUseFakeAuth(this IConfiguration config, string configPath = "FakeAuth:IsEnabled")
        => config[configPath]?.Equals("true", StringComparison.OrdinalIgnoreCase) ?? false;
}
