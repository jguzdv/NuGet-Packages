# JGUZDV.Blazor.WasmServerHost

This package is intended to be used as a helper to configure services and the request
pipeline needed to host a WASM aka Blazor application on an AspNetCore host.  
It is compatible with full client side WASM as well as more recent server side rendering with
interactivity via WASM.  

It's *not* intended for Blazor Server applciations, that use SingalR.

## Usage

**Program.cs**
```csharp
var builder = WebApplicationBuilder.Create(args);
builder.ConfigureWasmHostServices(useInteractiveWebAssembly: true);

var app = builder.Build();

app.ConfigueWasmHost<App>();
app.Run();
```

or with classic Blazor WASM
  
**Program.cs**
```csharp
var builder = WebApplicationBuilder.Create(args);
builder.ConfigureWasmHostServices(useInteractiveWebAssembly: false);

var app = builder.Build();

app.ConfigueWasmHost();
app.Run();
```

To use all features, you need to have configuration sections, each referring to a specific feature:

**appsettings.json**
```json
{
  "Logging": {
    ... // see https://nuget.org/jguzdv/JGUZDV.AspNetCore.Logging
  },
  
  "JGUZDV":{
    "DataProtection": {
    ... // see https://nuget.org/jguzdv/JGUZDV.AspNetCore.DataProtection
    },
  },

  "Authentication": {
    "OpenIdConnect": { ... },
    "Cookies": { ... }
  },

  "DistributedCache": {
    "ConnectionString": "...",
    "SchemaName": "...",
    "TableName": "..."
  },

  "ReverseProxy": {
    "Proxies": [
      {
        "PathMatch": "/api/{**catch-all}",
        "UpstreamUrl": "https://upstream.example/",
        "PathPrefix": "/api",
        "UseAccessToken": true
      }
    ]

    ... // see https://nuget.org/jguzdv/JGUZDV.YARP.SimpleReverseProxy
  },
}
```