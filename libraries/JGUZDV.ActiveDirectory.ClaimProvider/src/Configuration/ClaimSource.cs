using System.Diagnostics.CodeAnalysis;

namespace JGUZDV.ActiveDirectory.ClaimProvider.Configuration;

public record ClaimSource(string ClaimType, string PropertyName)
{
    public List<string>? ClaimValueDenyList { get; set; }
}
