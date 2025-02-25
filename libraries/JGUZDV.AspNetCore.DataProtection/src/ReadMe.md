# JGUZDV.AspNetCore.DataProtection

This package allows to configure data protection via the `appsettings.json` file (or the AspNetCore configuration system to be more precise).
If you want to configure multiple applications running on a shared host, you might want to set those keys via the environment variables.
E.g. `DataProtection__Persistence__FileSystem__Path`.

It can be addded via the extension method `AddJGUZDVDataProtection` on either `IServiceCollection` or `WebApplicationBuilder`.

**Program.cs**
```
// Registeres the DataProtection settings in production
if (builder.Environment.IsProduction())
{
    builder.AddJGUZDVDataProtection();
}
```

**appsettings.json**
```jsonc
{ 
  "DataProtection" : {    // default section JGUZDV:DataProtection - a custom section can be passed
    "ApplicationDiscriminator" : "MyApp",    // Name to be used by SetApplicationName
    "DisableAutomaticKeyGeneration" : false    // if true automatic key generation is disabled, default: false

    "Persistence" : {    // must be defined
      "FileSystem" : {    // must be defined
        "Path" : "/my-dataprot-path/",    // path for file system persistence
        "IsolatedPathDiscriminator" : true    // if not set, IHostingEnvironment.ApplicationName will be used as default
      }
    },

    "UseProtection" : true    // if true, protect keys, default: true
    "Protection" : {    // must be defined if UseProtection is true
      "UseCertificate" : false,    // if true, use certificate for protection
      "Certificate" : {    // optional, must be defined if UseCertificate is true
        "Thumbprint" : "",    // certificate thumbprint - must be set if file name is empty
        "FileName" : "name",    // certificate filename - must be set if thumbprint empty
        "Password" : "password"    // optional password for certificate
      },
      "UseDpapi" : false,    // if true, use Windows DPAPI
      "UseDpapiNG" : false    // if true, use Windows Server DpapiNG
    }
  }
}
```