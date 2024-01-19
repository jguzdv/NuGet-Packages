using Microsoft.Extensions.Primitives;

using Yarp.ReverseProxy.Configuration;

namespace JGUZDV.YARP.SimpleReverseProxy.Configuration
{
    internal class ReverseProxyInMemoryConfig : IProxyConfig
    {
        private readonly CancellationTokenSource _cts;

        public ReverseProxyInMemoryConfig(IReadOnlyList<RouteConfig> routes, IReadOnlyList<ClusterConfig> clusters)
        {
            Routes = routes;
            Clusters = clusters;

            _cts = new CancellationTokenSource();
            ChangeToken = new CancellationChangeToken(_cts.Token);
        }

        public IChangeToken ChangeToken { get; }

        public IReadOnlyList<RouteConfig> Routes { get; }
        public IReadOnlyList<ClusterConfig> Clusters { get; }

        public void SignalChange()
        {
            _cts.Cancel();
        }
    }
}
