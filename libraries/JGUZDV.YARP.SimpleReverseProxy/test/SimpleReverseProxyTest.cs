using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Logging;

using System.Net;

namespace JGUZDV.YARP.SimpleReverseProxy.Tests;

public class SimpleReverseProxyTest
{
    [Fact]
    public async Task SimpleProxy()
    {
        using var host = new WebHostBuilder()
            .UseUrls("http://localhost:12100")
            .UseKestrel()
#if DEBUG
            .ConfigureLogging(log => { log.AddDebug(); })
#endif
            .Configure(app =>
            {
                app.Map("/app", hw => hw.Run((ctx) => ctx.Response.WriteAsync("Hello World!")));
            })
            .Build();

        await host.StartAsync();

        using var proxyHost = new WebHostBuilder()
            .UseTestServer(opt =>
            {
                opt.BaseAddress = new Uri("http://localhost:12200");
            })
#if DEBUG
            .ConfigureLogging(log => { log.AddDebug(); })
#endif
            .ConfigureServices((ctx, services) =>
            {
                services.AddSimpleReverseProxy(null,
                    bff =>
                    {
                        bff.Proxies.Add(
                            new Configuration.SimpleProxyDefinition(
                                "/proxy/path",
                                "http://localhost:12100/app"
                            ));
                    });
            })
            .Configure(app =>
            {
                app.UseRouting();
                app.UseEndpoints(e => e.MapSimpleReverseProxy());
            })
            .Build();

        await proxyHost.StartAsync();

        var response = await proxyHost.GetTestServer().CreateClient().GetAsync("/proxy/path");
        var result = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("Hello World!", result);
    }


    [Fact]
    public async Task PrefixRequestsWillBeProxied()
    {
        using var host = new WebHostBuilder()
            .UseUrls("http://localhost:12101")
            .UseKestrel()
#if DEBUG
            .ConfigureLogging(log =>
            {
                log.AddDebug();
            })
#endif
            .ConfigureServices(services =>
            {
            })
            .Configure(app =>
            {
                app.Map("/app/app-path", hw => hw.Run((ctx) => ctx.Response.WriteAsync("Hello World!")));
            })
            .Build();

        await host.StartAsync();

        using var proxyHost = new WebHostBuilder()
            .UseTestServer(opt =>
            {
                opt.BaseAddress = new Uri("http://localhost:12201");
            })
#if DEBUG
            .ConfigureLogging(log => log.AddDebug())
#endif
            .ConfigureServices((ctx, services) =>
            {
                services.AddSimpleReverseProxy(null,
                    bff =>
                    {
                        bff.Proxies.Add(
                            new Configuration.SimpleProxyDefinition
                            (
                                "/proxy/path/subpath/{**catch-all}",
                                "http://localhost:12101/app"
                            )
                            {
                                PathPrefix = "/proxy/path/subpath",
                            });
                    });
            })
            .Configure(app =>
            {
                app.UseRouting();
                app.UseEndpoints(e => e.MapSimpleReverseProxy());
            })
            .Build();

        await proxyHost.StartAsync();

        var response = await proxyHost.GetTestServer().CreateClient().GetAsync("/proxy/path/subpath/app-path");
        var result = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("Hello World!", result);
    }
}