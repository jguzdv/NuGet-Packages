
```json
	{
		"JGUZDV" : {
			"DataProtection" : {                       // default section JGUZDV:DataProtection - a custom section can be passed
				"ApplicationName" : "Name",            // Name to be used by SetApplicationName and UsePathIsolation
				"SetApplicationName" : true,           // if true, calls SetApplicationName and passes ApplicationName
				"DisableAutomaticKeyGeneration" : true // if true calls 

				"UsePersistence" : true				   // if true, uses persistence
				"Persistence" : {					   // optional, must be defined if UsePersistance is true
					"UseFileSystem" : true,            // if true, persists to file system
					"FileSystem" : {				   // optional, must be defined if UseFileSystem is true
						"Path" : "/path/",			   // path for file system persistence
						"UseIsolatedPath" : true	   // if true, combines Path with ApplicationName
					}
				},

				"UseProtection" : true			       // if true, protect keys
				"Protection" : {					   // optional, must be defined if UseProtection is true
					"UseCertificate" : true,	       // if true, use certificate for protection
					"Certificate" : {                  // optional, must be defined if UseCertificate is true
						"Thumbprint" : "",			   // certificate thumbprint - must be set if file name is empty
						"FileName" : "name",	       // certificate filename - must be set if thumbprint empty
						"Password" : "password"        // optional password for certificate
					},
					"UseDpapi" : true,                 // if true, use DPAPI (Windows only)
					"UseDpapiNG" : true				   // if true, use DPApiNG (Windows domain only)
				}
			}
		}
	}
```