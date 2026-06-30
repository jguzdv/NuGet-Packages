namespace JGUZDV.AspNetCore.TestTools.FakeAuth;

/// <summary>
/// Contains default values for the FakeAuth authentication scheme, such as the default scheme name and the header name used to pass claims in requests.
/// </summary>
public static class FakeAuthDefaults
{
    /// <summary>
    /// The default authentication scheme name for FakeAuth. 
    /// This is the name that will be used when registering the authentication handler and when specifying the default authentication scheme in the application's authentication configuration.
    /// </summary>
    public static readonly string AuthenticationScheme = "FakeAuth";

    /// <summary>
    /// The name of the header used to pass claims in requests when using FakeAuth.
    /// </summary>
    public static readonly string ClaimsHeaderName = "X-Fake-Auth-Claims";
}
