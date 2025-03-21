using JGUZDV.Blazor.Hosting;
using JGUZDV.Blazor.Components;

var builder = JGUZDVWebAssemblyApplicationBuilder.CreateDefault(args);

builder.Services.AddToasts();

await builder.Build().RunAsync();