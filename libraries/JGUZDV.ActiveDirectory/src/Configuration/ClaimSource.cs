using System.Diagnostics.CodeAnalysis;

namespace JGUZDV.ActiveDirectory.Configuration;

/// <summary>
/// Represents a claim source.
/// </summary>
/// <param name="ClaimType">The type of the produced claim</param>
/// <param name="PropertyName">The property from which values are to be read from</param>
/// <param name="OutputFormat">An output formatter, that will either run a transform (see <see cref="OutputFormats"/>) or a string.Format()</param>
/// <param name="Casing">The casing to apply to the output</param>
public record ClaimSource(string ClaimType, string PropertyName, string? OutputFormat = null, Casing Casing = Casing.Unchanged)
{
    /// <summary>
    /// A list of claim values to filter from the result. Supports regex.
    /// </summary>
    public List<string>? ClaimValueDenyList { get; set; }
}
