using Microsoft.Extensions.Configuration;

namespace JGUZDV.AspNetCore.Hosting.Tests;

public class WebHostTests
{
    [Fact]
    public void Host_Will_Run_with_Little_Configuration()
    {
        var hostBuilder = JGUZDVHostApplicationBuilder.CreateWebApi([], ctx =>
        {
            var configs = new Dictionary<string, string?>
            {
                { "Logging:File:OutputDirectory", "."}
            };

            ctx.Configuration.AddInMemoryCollection(configs);
        });
        
            
        var host = hostBuilder.BuildAndConfigureDefault();

        _ = host.RunAsync();
        host.Lifetime.StopApplication();
    }
}
