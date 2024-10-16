# JGUZDV.WebHost

This package is intended to be used as a helper to configure services and the request
pipeline needed to host a AspNetCore WebAPI application.  
It's highly opinionated and will use our other packeges to provide a default configuration.

## Usage

**Program.cs**
```csharp
var builder = WebApplicationBuilder.Create(args);
builder.ConfigureWebApiHost(); 

var app = builder.Build();

app.ConfigureWebHost();
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
    "OpenIdConnect": {
      "Authority": "{AuthorityAdress}",
      "ClientId": "{ClientId}",
      "ClientSecret": "{ClientSecret}",
      "ResponseType": "{type}",
      "Scope": [ {"ListOfScopes"} ]
    }
  },

  "DistributedCache": { // Omit if not needed
    "ConnectionString": "...",
    "SchemaName": "...",
    "TableName": "..."
  },
  "ApplicationInsights": {
    "ConnectionString": "InstrumentationKey={keyValue}};IngestionEndpoint={EndpointAddress}}"
  },
}
```