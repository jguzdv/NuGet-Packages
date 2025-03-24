using JGUZDV.Blazor.Hosting;
using JGUZDV.Blazor.Components;

var builder = JGUZDVWebAssemblyApplicationBuilder.CreateDefault(args);

await builder.Build().RunAsync();