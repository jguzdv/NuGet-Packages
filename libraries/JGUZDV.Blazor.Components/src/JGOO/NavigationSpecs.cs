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

