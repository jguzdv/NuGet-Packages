using JGUZDV.AspNetCore.TestTools.FakeAuth;

using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection;

public static class FakeAuthenticationBuilderExtensions
{
    public static AuthenticationBuilder AddFakeAuth(this AuthenticationBuilder builder,
        Action<FakeAuthenticationOptions>? setupAction = null)
    {
        builder.AddScheme<FakeAuthenticationOptions, FakeAuthenticationHandler>(FakeAuthDefaults.AuthenticationScheme, setupAction ?? (_ => { }));

        return builder;
    }

    public static AuthenticationBuilder AddFakeAuth(this AuthenticationBuilder builder,
        string? authenticationScheme = null,
        Action<FakeAuthenticationOptions>? setupAction = null)
    {
        builder.AddScheme<FakeAuthenticationOptions, FakeAuthenticationHandler>(authenticationScheme ?? FakeAuthDefaults.AuthenticationScheme, setupAction ?? (_ => { }));

        return builder;
    }
}

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

    public static bool ShouldUseFakeAuth(this IConfiguration config, string configPath = "FakeAuth:IsEnabled")
        => config[configPath]?.Equals("true", StringComparison.OrdinalIgnoreCase) ?? false;
}
