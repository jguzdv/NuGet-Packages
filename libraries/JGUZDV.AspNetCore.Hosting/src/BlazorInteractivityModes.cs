namespace JGUZDV.AspNetCore.Hosting;

/// <summary>
/// Flags for the interactivity modes of Blazor.
/// </summary>
[Flags]
public enum BlazorInteractivityModes
{
    /// <summary>
    /// Blazor will not be configured.
    /// </summary>
    DisableBlazor = 0,

    /// <summary>
    /// No interactivity is enabled (Blazor Static Server).
    /// </summary>
    None = 1,

    /// <summary>
    /// Interactive server components are enabled (Blazor Server).
    /// </summary>
    Server = 2,

    /// <summary>
    /// Interactive client components are enabled (Blazor WebAssembly).
    /// </summary>
    WebAssembly = 4,

    /// <summary>
    /// All interactivity modes are enabled.
    /// </summary>
    Auto = Server | WebAssembly,
}