using JGUZDV.Blazor.Hosting;

var builder = JGUZDVWebAssemblyApplicationBuilder.CreateDefault(args);
await builder.Build().RunAsync();