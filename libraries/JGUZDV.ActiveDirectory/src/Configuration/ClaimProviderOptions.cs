namespace JGUZDV.ActiveDirectory.Configuration;

/// <summary>
/// Claim provider configuration options.
/// </summary>
public class ClaimProviderOptions
{
    public List<ClaimSource> ClaimSources { get; set; } = Defaults.KnownClaimSources.ToList();  
}
