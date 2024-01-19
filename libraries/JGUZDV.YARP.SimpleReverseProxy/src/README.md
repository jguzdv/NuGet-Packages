# JGUZDV.YARP.SimpleReverseProxy

This is a simplification of YARPs configuration build for "BFFs" (Backend for Frontend).
It facilitates [YARP]() and [IdentityModel.AspNetCore]() to forward requests to a backend system,
that uses Bearer Tokens for authentication and will automatically include those tokens in requests 
to the aforementioned.

## Usage

**Program.cs**
```csharp
var builder = WebApplication.CreateBuilder(args);
builder.AddSimpleReverseProxy("ReverseProxy");

var app = builer.Build();
app.MapSimpleReverseProxy();

app.Run();
```

**appsettings.json**
```csharp
{
  "ReverseProxy":{
    "Proxies": [{
      "PathMatch": "api/some/**requestPath",
      "PathPrefix": "api/", // Remove from upstream path

      "UpstreamUrl": "https://upstream.url/path", // would result in https://upstream.url/path/some/**requestPath
      "UseAccessToken": [true|false]
    }]
  }
}
```