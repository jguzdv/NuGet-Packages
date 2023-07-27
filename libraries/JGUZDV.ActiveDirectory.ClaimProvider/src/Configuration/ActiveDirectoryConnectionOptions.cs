using System.Diagnostics.CodeAnalysis;

namespace JGUZDV.ActiveDirectory.ClaimProvider.Configuration;

public class ActiveDirectoryConnectionOptions
{
    [NotNull]
    public string? Server { get; set; }
    public string? BaseDN { get; set; }
}
