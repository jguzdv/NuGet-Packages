using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace JGUZDV.Blazor.Components.JGOO;

[Obsolete("Use AppHeader.NavItem or Sidebar.NavItem components instead. This will be removed in a future version.")]
public class NavigationSpecs
{
    public RenderFragment? Icon { get; set; }
    public string? NavUrl { get; set; }
    public required string Name { get; set; }
    public NavLinkMatch Match { get; set; } = NavLinkMatch.Prefix;

    public List<NavigationSpecs>? Children { get; set; }

    public bool HasChildren => Children != null && Children.Any();
}

/// <summary>
/// Provides rendering context information for navigation components to determine
/// how they should render based on their location and nesting level.
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
