namespace JGUZDV.Blazor.Components.JGOO;

/// <summary>
/// Provides rendering context information for navigation components
/// </summary>
public class NavRenderContext
{
    /// <summary>
    /// Gets the nesting level (0 = top-level, 1 = first nested level, etc.).
    /// </summary>
    public int NestingLevel { get; init; }

    /// <summary>
    /// Creates a nested context from a parent context.
    /// </summary>
    public static NavRenderContext CreateNestedContext(NavRenderContext parent) => new()
    {
        NestingLevel = parent.NestingLevel + 1,
    };

    /// <summary>
    /// Creates a root context for top-level navigation items.
    /// </summary>
    /// <returns></returns>
    public static NavRenderContext CreateRootContext() => new()
    {
        NestingLevel = 0,
    };
}
