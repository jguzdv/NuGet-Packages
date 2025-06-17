using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ZDV.BlazorWebTemplate.WebApi.Tests;

public class DefaultWebApplicationFactory
    : WebApplicationFactory<Program>, IDisposable
{
    public DefaultWebApplicationFactory()
    {
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test");

        builder.ConfigureAppConfiguration((ctx, config) =>
        {
            // You can hardcode config here
            var webConfig = new Dictionary<string, string?>()
            {
            };

            config.AddInMemoryCollection(webConfig);
        });

        builder.ConfigureServices(services =>
        {
            // Configure Fake Services here, if neccessary for testing
        });

        // Setup and initialize a database if necessary for testing
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        // Database.EnsureDeleted();
    }
}