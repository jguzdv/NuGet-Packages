using JGUZDV.Blazor.Hosting;
using JGUZDV.Blazor.Components;

var builder = JGUZDVWebAssemblyApplicationBuilder.CreateDefault(args);

var host = await builder.BuildAsync();
await host.RunAsync();