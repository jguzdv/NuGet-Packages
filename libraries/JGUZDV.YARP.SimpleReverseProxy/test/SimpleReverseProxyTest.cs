using System.Net;

using Microsoft.AspNetCore.TestHost;

namespace JGUZDV.YARP.SimpleReverseProxy.Tests;

public class SimpleReverseProxyTest :
    IClassFixture<ReverseProxyWebApplicationFactory>
{
    private readonly ReverseProxyWebApplicationFactory _webFactory;

    public SimpleReverseProxyTest(ReverseProxyWebApplicationFactory webFactory)
    {
        _webFactory = webFactory;
    }

    [Fact]
    public async Task Simple_Gets_are_Proxied()
    {
        var client = _webFactory.ReverseProxyServer.GetTestClient();

        var helloWorldResponse = await client.GetAsync("/proxy/path/subpath/something");
        var helloWorld = await helloWorldResponse.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, helloWorldResponse.StatusCode);
        Assert.Equal("Hello World!", helloWorld);
    }

    [Theory, 
        InlineData("en-US", "en-US"),
        InlineData("de-DE", "de-DE"),
        InlineData("fr-FR", "fr-FR"),
        InlineData("pt-PT", "de-DE")]
    public async Task Language_Is_Proxied(string culture, string expectedResponse)
    {
        var client = _webFactory.ReverseProxyServer.GetTestClient();

        var uri = new Uri("/proxy/path/subpath/culture", UriKind.Relative);
        var acceptLanguageRequest = new HttpRequestMessage(HttpMethod.Get, uri);
        acceptLanguageRequest.Headers.AcceptLanguage.Clear();
        acceptLanguageRequest.Headers.AcceptLanguage.Add(new(culture));

        var acceptLanguageResponse = await client.SendAsync(acceptLanguageRequest);
        var responseText = await acceptLanguageResponse.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, acceptLanguageResponse.StatusCode);
        Assert.Equal(expectedResponse, responseText);
    }
}