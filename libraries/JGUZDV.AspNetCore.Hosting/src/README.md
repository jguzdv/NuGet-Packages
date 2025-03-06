# JGUZDV.AspNetCore.Hosting

This library provides specialized wrappers around the WebHostBuilder and WebAssemblyHostBuilder classes to simplify the configuration of ASP.NET Core applications in our environment.

## Usage

Create a host:
```csharp
using JGUZDV.AspNetCore.Hosting;

var builder = JGUZDVHostApplicationBuilder.Create(args);

builder.UseWebApiDefaults();
builder.UseWebDefaults();
builder.UseWebAssemblyHostDefaults();

var app = builder.BuildAndConfigureDefault();

app.MapGet("/", () => "Hello, World!");

```