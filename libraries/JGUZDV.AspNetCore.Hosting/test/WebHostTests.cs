using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;

namespace JGUZDV.AspNetCore.Hosting.Tests;

public class WebHostTests
{
    [Fact]
    public void Host_Will_Run_with_Little_Configuration()
    {
        var hostBuilder = JGUZDVHostApplicationBuilder.CreateWebApi(["--environment", "Test"], ctx =>
        {
            ctx.Builder.WebHost.UseTestServer();

            //var config = new Dictionary<string, string?>
            //{
            //    { "Logging:File:OutputDirectory", "." }
            //};

            //ctx.Configuration.AddInMemoryCollection(config);
        });
        
            
        var host = hostBuilder.BuildAndConfigureDefault();

        _ = host.RunAsync();
        host.Lifetime.StopApplication();
    }
}
