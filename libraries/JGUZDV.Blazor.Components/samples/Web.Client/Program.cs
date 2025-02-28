using System.Reflection.PortableExecutable;

using JGUZDV.Blazor.Hosting;

var builder = JGUZDVWebAssemblyApplicationBuilder.CreateDefault(args);

builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthenticationStateDeserialization();

await builder.Build().RunAsync();