
using Yarp.ReverseProxy.Forwarder;

namespace JGUZDV.YARP.SimpleReverseProxy.Tests
{
    internal class TestForwarderHttpClientFactory : IForwarderHttpClientFactory
    {
        private readonly HttpMessageHandler _httpMessageHandler;

        public TestForwarderHttpClientFactory(HttpMessageHandler httpMessageHandler)
        {
            _httpMessageHandler = httpMessageHandler;
        }

        public HttpMessageInvoker CreateClient(ForwarderHttpClientContext context)
        {
            return new HttpMessageInvoker(_httpMessageHandler);
        }
    }
}