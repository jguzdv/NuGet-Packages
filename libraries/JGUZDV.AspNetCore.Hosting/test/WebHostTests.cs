using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;

namespace JGUZDV.AspNetCore.Hosting.Tests;

public class WebHostTests
{
    [Fact]
    public void Host_Will_Run_with_Little_Configuration()
    {
        var hostBuilder = JGUZDVHostApplicationBuilder.CreateWebApi([], ctx =>
        {
            ctx.Builder.WebHost.UseTestServer();

            var config = new Dictionary<string, string?>
    {
        { "CORS:Policy1:Headers:0", "Content-Length" },
        { "CORS:Policy1:Headers:1", "Content-Type" },
        { "CORS:Policy1:Methods:0", "GET" },
        { "CORS:Policy1:Methods:1", "POST" },
        { "CORS:Policy1:Origins:0", "https://example.com" },
        { "CORS:Policy1:Origins:1", "https://another.com" },
        { "CORS:Policy1:SupportsCredentials", "true" }
    };

            ctx.Configuration.AddInMemoryCollection(config);
        });


        var host = hostBuilder.BuildAndConfigureDefault();

        _ = host.RunAsync();
        host.Lifetime.StopApplication();
    }
}
