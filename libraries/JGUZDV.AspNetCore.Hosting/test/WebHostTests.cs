namespace JGUZDV.AspNetCore.Hosting.Tests;

public class WebHostTests
{
    [Fact]
    public void Host_Will_Run_without_Configuration()
    {
        var host = JGUZDVHostApplicationBuilder.CreateWebApi([]).BuildAndConfigureDefault();

        _ = host.RunAsync();
        host.Lifetime.StopApplication();
    }
}
