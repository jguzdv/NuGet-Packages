using System.Diagnostics.CodeAnalysis;

namespace JGUZDV.YARP.SimpleReverseProxy.Configuration;

[method: SetsRequiredMembers]
public class SimpleProxyDefinition(string pathMatch, string upstreamUrl)
{
    public required string PathMatch { get; set; } = pathMatch;
    public string? PathPrefix { get; set; }

    public required string UpstreamUrl { get; set; } = upstreamUrl;
    public bool UseAccessToken { get; set; }
}
