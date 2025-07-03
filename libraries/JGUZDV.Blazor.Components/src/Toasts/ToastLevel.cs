namespace JGUZDV.Blazor.Components.Toasts;

/// <summary>
/// Severity levels for toast messages.
/// </summary>
public enum ToastLevel
{
    /// <summary>
    /// Default level, no specific styling applied.
    /// </summary>
    None,

    /// <summary>
    /// Information level, blue styling typically applied.
    /// </summary>
    Info,

    /// <summary>
    /// Success level, green styling typically applied.
    /// </summary>
    Success,

    /// <summary>
    /// Warning level, yellow styling typically applied.
    /// </summary>
    Warning,

    /// <summary>
    /// Error level, red styling typically applied.
    /// </summary>
    Danger
}
