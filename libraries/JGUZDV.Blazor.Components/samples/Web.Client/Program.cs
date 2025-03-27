using JGUZDV.Blazor.Hosting;
using JGUZDV.Blazor.Components;

var builder = JGUZDVWebAssemblyApplicationBuilder.CreateDefault(args);

builder.Logging.SetMinimumLevel(LogLevel.Debug);

var host = await builder.BuildAsync();
await host.RunAsync();