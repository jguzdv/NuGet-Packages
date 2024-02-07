using Microsoft.Extensions.Options;

using Yarp.ReverseProxy.Configuration;
using Yarp.ReverseProxy.Transforms;

namespace JGUZDV.YARP.SimpleReverseProxy.Configuration;

public class SimpleReverseProxyConfigProvider : IProxyConfigProvider
{
    private readonly IOptionsMonitor<SimpleReverseProxyOptions> _options;
    private ReverseProxyInMemoryConfig? _proxyConfig;

    public SimpleReverseProxyConfigProvider(IOptionsMonitor<SimpleReverseProxyOptions> options)
    {
        _options = options;
        UpdateConfig(_options.CurrentValue);

        _options.OnChange(UpdateConfig);
    }

    private void UpdateConfig(SimpleReverseProxyOptions options)
    {
        var oldConfig = _proxyConfig;

        try
        {
            _proxyConfig = GetConfigFromOptions(options);
            oldConfig?.SignalChange();
        }
        catch
        {
            //TODO: Logging
        }
    }

    private ReverseProxyInMemoryConfig GetConfigFromOptions(SimpleReverseProxyOptions options)
    {
        var routes = new List<RouteConfig>();
        var clusters = new List<ClusterConfig>();

        foreach (var proxy in options.Proxies)
        {
            var clusterId = Guid.NewGuid().ToString();
            var routeId = Guid.NewGuid().ToString();

            var upstreamUri = new Uri(proxy.UpstreamUrl);

            var cluster = new ClusterConfig
            {
                ClusterId = clusterId,
                Destinations = new Dictionary<string, DestinationConfig>
                {
                    { "Default", new() { Address = upstreamUri.ToString() } }
                }
            };

            var route = new RouteConfig
            {
                RouteId = routeId,
                ClusterId = clusterId,
                Match = new() { Path = proxy.PathMatch },
                Metadata = new Dictionary<string, string>
                {
                    { nameof(proxy.UseAccessToken), $"{proxy.UseAccessToken}" }
                }
            };

            if (!string.IsNullOrWhiteSpace(proxy.PathPrefix))
                route = route.WithTransformPathRemovePrefix(proxy.PathPrefix);

            routes.Add(route);
            clusters.Add(cluster);
        }

        return new ReverseProxyInMemoryConfig(routes, clusters);
    }

    public IProxyConfig GetConfig() => _proxyConfig ?? new ReverseProxyInMemoryConfig([], []);
}
