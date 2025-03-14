using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Yarp.ReverseProxy.Forwarder;

namespace JGUZDV.YARP.SimpleReverseProxy.Tests;

public class ReverseProxyWebApplicationFactory
    : IDisposable
{
    public IHost TargetServer { get; }
    public IHost ReverseProxyServer { get; }


    public ReverseProxyWebApplicationFactory()
    {
        TargetServer = CreateTargetServer();
        ReverseProxyServer = CreateReverseProxyServer(TargetServer.GetTestServer().CreateHandler());
    }

    private IHost CreateReverseProxyServer(HttpMessageHandler httpMessageHandler)
    {
        return new HostBuilder()
#if DEBUG
           .ConfigureLogging(log => log.AddDebug())
#endif
           .ConfigureWebHost(webBuilder =>
           {
               webBuilder
                   .UseTestServer()
                   .ConfigureServices((ctx, services) =>
                     {
                         services.AddRequestLocalization(opt =>
                         {
                             opt.AddSupportedCultures("en-US", "de-DE", "fr-FR");
                             opt.AddSupportedUICultures("en-US", "de-DE", "fr-FR");
                             opt.DefaultRequestCulture = new RequestCulture("de-DE");
                         });

                         services.AddSimpleReverseProxy(null,
                             bff =>
                             {
                                 bff.Proxies.Add(
                                     new Configuration.SimpleProxyDefinition
                                     (
                                         "/proxy/path/subpath/{**catch-all}",
                                         "http://localhost/app"
                                     )
                                     {
                                         PathPrefix = "/proxy/path/subpath",
                                     });
                             });

                         services.AddSingleton<IForwarderHttpClientFactory, TestForwarderHttpClientFactory>(sp => new TestForwarderHttpClientFactory(httpMessageHandler));
                     })
                    .Configure(app =>
                    {
                        app.UseRequestLocalization();
                        app.UseRouting();
                        app.UseEndpoints(e => {
                            e.MapSimpleReverseProxy();
                            e.MapFallback((ctx) => ctx.Response.WriteAsync("Fell through"));
                        });
                    });
           })
           .Start();
    }

    private IHost CreateTargetServer()
    {
        return new HostBuilder()
#if DEBUG
           .ConfigureLogging(log => log.AddDebug())
#endif
            .ConfigureWebHost(webBuilder =>
            {
                webBuilder
                    .UseTestServer()
                    .ConfigureServices(services =>
                    {
                        services.AddRequestLocalization(opt =>
                        {
                            opt.AddSupportedCultures("en-US", "de-DE", "fr-FR");
                            opt.AddSupportedUICultures("en-US", "de-DE", "fr-FR");
                            opt.DefaultRequestCulture = new RequestCulture("de-DE");
                        });
                        services.AddRouting();
                    })
                    .Configure((ctx, app) =>
                    {
                        app.UseRequestLocalization();
                        app.UseRouting();
                        app.UseEndpoints(endpoints =>
                        {
                            // Simple endpoint to test the reverse proxy
                            endpoints.MapGet("app/something", (ctx) => ctx.Response.WriteAsync("Hello World!"));
                            endpoints.MapGet("app/culture", (ctx) => ctx.Response.WriteAsync(ctx.Features.Get<IRequestCultureFeature>()?.RequestCulture.UICulture.Name ?? "none"));
                        });

                    });
            })
            .Start();
    }


    public void Dispose()
    {
        TargetServer?.Dispose();
        ReverseProxyServer?.Dispose();
    }
}
