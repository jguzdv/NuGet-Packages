using Microsoft.AspNetCore.Authentication;

namespace JGUZDV.AspNetCore.TestTools.FakeAuth;

/// <summary>
/// Options for the FakeAuthenticationHandler. Contains a list of allowed environments and claims to be added to the user.
/// </summary>
public class FakeAuthenticationOptions : AuthenticationSchemeOptions
{
    /// <summary>
    /// List of allowed environments for the FakeAuthenticationHandler. If the current environment is not in this list, the handler will throw an exception.
    /// </summary>
    public string[] AllowedEnvironments { get; set; } = ["Development", "Test"];

    /// <summary>
    /// List of claims to be added to the user. These claims will be added to the user on every request.
    /// You can also add claims to the user by adding a header to the request with the name "X-Fake-Auth-Claims" and the value "Type::Value".
    /// This will add a claim with the specified type and value to the user for that request only.
    /// </summary>
    public List<FakeClaim> Claims { get; set; } = new();

    /// <summary>
    /// The name of the header to be used for adding claims to the user.
    /// The default value is "X-Fake-Auth-Claims". You can change this if you want to use a different header name for adding claims to the user.
    /// </summary>
    public string ClaimHeaderName { get; set; } = FakeAuthDefaults.ClaimsHeaderName;
}
