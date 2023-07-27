using System.Diagnostics.CodeAnalysis;

namespace JGUZDV.ActiveDirectory.ClaimProvider.Configuration;

public class ClaimSource
{
    [NotNull]
    public string? ClaimType { get; set; }

    [NotNull]
    public string? PropertyName { get; set; }


    public string? PropertyConverter { get; set; }

    public List<string>? ClaimValueDenyList { get; set; }
}
