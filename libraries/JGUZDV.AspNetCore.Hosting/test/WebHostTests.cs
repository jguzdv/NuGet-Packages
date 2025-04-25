using System.Security.Claims;
using System.Text.Encodings.Web;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

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
