using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
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
            .ConfigureServices(services =>
            {
                services.AddRouting();
            })
            .Configure(app =>
            {
                app.UseRouting();
                app.UseEndpoints(ep => {

                    ep.MapGet("/app", (ctx) => ctx.Response.WriteAsync("Hello World!"));
                    ep.MapGet("/accept-language", (ctx) => ctx.Response.WriteAsync(ctx.Request.Headers.AcceptLanguage.FirstOrDefault()?.ToString() ?? ""));
                });
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
                services.AddRequestLocalization(x => x.SetDefaultCulture("thl-XK"));

                services.AddSimpleReverseProxy(null,
                    bff =>
                    {
                        bff.Proxies.Add(
                            new Configuration.SimpleProxyDefinition(
                                "/proxy/path",
                                "http://localhost:12100/app"
                            ));
                        
                        bff.Proxies.Add(
                            new Configuration.SimpleProxyDefinition(
                                "/proxy/lng",
                                "http://localhost:12100/accept-language"
                            ));
                    });
            })
            .Configure(app =>
            {
                app.UseRequestLocalization();
                app.UseRouting();
                app.UseEndpoints(e => e.MapSimpleReverseProxy());
            })
            .Build();

        await proxyHost.StartAsync();

        var helloWorldResponse = await proxyHost.GetTestServer().CreateClient().GetAsync("/proxy/path");
        var helloWorld = await helloWorldResponse.Content.ReadAsStringAsync();
        
        var acceptLanguageResponse = await proxyHost.GetTestServer().CreateClient().GetAsync("/proxy/lng");
        var acceptLanguage = await acceptLanguageResponse.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, helloWorldResponse.StatusCode);
        Assert.Equal("Hello World!", helloWorld);
        
        Assert.Equal(HttpStatusCode.OK, acceptLanguageResponse.StatusCode);
        Assert.Equal("thl-XK; q=1.0", acceptLanguage);
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