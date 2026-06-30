using System.Security.Claims;
using System.Text.Encodings.Web;

using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace JGUZDV.AspNetCore.TestTools.FakeAuth;

/// <summary>
/// Authentication handler for the fake authentication scheme. This is meant to be used in test cases only and will throw an exception if called in an environment where it is not allowed.
/// </summary>
public class FakeAuthenticationHandler : AuthenticationHandler<FakeAuthenticationOptions>
{
    private readonly IHostEnvironment _environment;

    /// <summary>
    /// Initializes a new instance of the <see cref="FakeAuthenticationHandler"/> class.
    /// </summary>
    public FakeAuthenticationHandler(IHostEnvironment environment, 
        IOptionsMonitor<FakeAuthenticationOptions> options, 
        ILoggerFactory logger, UrlEncoder encoder)
        : base(options, logger, encoder)
    {
        _environment = environment;
    }

    /// <inheritdoc />
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var identitiyClaims = Options.Claims
            .Select(x => new Claim(x.Type, x.Value))
            .ToList();

        var headerClaims = Request.Headers.ReadClaimsHeader();
        identitiyClaims.AddRange(headerClaims.Select(x => new Claim(x.Type, x.Value)));

        if (_environment.IsProduction() || !Options.AllowedEnvironments.Any(_environment.IsEnvironment))
        {
            Logger.LogCritical("Fake Authentication Handler has been called in an environment where it was not allowed.");
            throw new InvalidOperationException("Fake Authentication is meant for test cases only.");
        }

        Logger.LogWarning("Fake identity will be created now.");
        var identity = new ClaimsIdentity(
            identitiyClaims,
            Scheme.Name,
            "displayName", "role");

        var authenticationTicket = new AuthenticationTicket(
            new ClaimsPrincipal(identity),
            new AuthenticationProperties(),
            Scheme.Name);

        return Task.FromResult(AuthenticateResult.Success(authenticationTicket));
    }
}
