namespace JGUZDV.AspNetCore.TestTools.FakeAuth;

/// <summary>
/// Represents a claim with a type and value, used for simulating authentication scenarios in testing and development environments.
/// </summary>
public record FakeClaim(string Type, string Value);