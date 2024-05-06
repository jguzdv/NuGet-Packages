using System.Diagnostics.CodeAnalysis;

namespace JGUZDV.ActiveDirectory.Configuration;

/// <summary>
/// Represents a claim source.
/// </summary>
/// <param name="ClaimType"></param>
/// <param name="PropertyName"></param>
/// <param name="OutputFormat"></param>
public record ClaimSource(string ClaimType, string PropertyName, string? OutputFormat = null)
{
    /// <summary>
    /// A list of claim values to filter from the result. Supports regex.
    /// </summary>
    public List<string>? ClaimValueDenyList { get; set; }
}
