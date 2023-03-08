namespace JGUZDV.Blazor.Components.Authentication;

public class HostAuthenticationOptions
{
    public int PollIntervalSeconds { get; set; } = 120;
    public int CacheRefreshIntervalSeconds { get; set; } = 3600;

    public string NameClaimType { get; set; } = "displayName";
    public string RoleClaimType { get; set; } = "role";
}
