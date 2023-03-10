# JGUZDV.AspNetCore.DataProtection

This package allows to configure data protection via the `appsettings.json` file (or the AspNetCore configuration system to be more precise).
If you want to configure multiple applications running on a shared host, set those keys via the environment variables. E.g. `JGUZDV__DataProtection__UsePersistance`.

```jsonc
{ 
  "JGUZDV" : {
    "DataProtection" : {    // default section JGUZDV:DataProtection - a custom section can be passed
      "ApplicationName" : "MyApp",    // Name to be used by SetApplicationName and UsePathIsolation, if not set, IHostingEnvironment.ApplicationName will be used as default.
      "SetApplicationName" : false,    // if true, calls SetApplicationName and passes the aforementioned ApplicationName, default: false
      "DisableAutomaticKeyGeneration" : false    // if true automatic key generation is disabled, default: false

      "UsePersistence" : true    // enables persistence configuration, default: true
      "Persistance" : {    // must be defined if UsePersistance is true
        "UseFileSystem" : false,    // if true, persists to file system, default: false
        "FileSystem" : {    // must be defined if UseFileSystem is true
          "Path" : "/my-dataprot-path/",    // path for file system persistence
          "UseIsolatedPath" : true    // if true, combines Path with ApplicationName, e.g. /my-dataprot-path/MyApp, default: true
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
        "UseDpapi" : true,    // if true, use Windows DPAPI
        "UseDpapiNG" : true    // if true, use Windows Server DpapiNG
      }
    }
  }
}
```