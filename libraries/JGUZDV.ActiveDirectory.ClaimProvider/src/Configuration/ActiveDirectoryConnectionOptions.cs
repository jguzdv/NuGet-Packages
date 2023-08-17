using System.Diagnostics.CodeAnalysis;

namespace JGUZDV.ActiveDirectory.ClaimProvider.Configuration;

public class ActiveDirectoryConnectionOptions
{
    private string _server = "";

    [AllowNull]
    public string Server {
        get => _server;
        set
        {
            _server = string.IsNullOrWhiteSpace(value) ? "" : value;

            if (!_server.EndsWith("/"))
                _server += "/";
        }
    }

    public string? BaseDN { get; set; }
}
