namespace JGUZDV.ActiveDirectory.Configuration;

/// <summary>
/// A casing to be applied to a claim value.
/// </summary>
public enum Casing
{
    /// <summary>
    /// Keep the casing as it is.
    /// </summary>
    Unchanged,

    /// <summary>
    /// Apply lower casing to the output.
    /// </summary>
    Lower,

    /// <summary>
    /// Apply upper casing to the output.
    /// </summary>
    Upper
}