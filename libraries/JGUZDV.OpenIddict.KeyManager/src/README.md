# JGUZDV.OpenIddict.KeyManager

This package provides functions to automatically rollover signature and encryption keys used in OpenIddict.
It'll store the keys to a harddrive or share, so you can leverage whatever synchronization mechanisms you have in place.

```csharp
/* Program.cs */

services.AddAutomaticKeyRollover(conf =>
{
    conf.KeyStorePath = "C:\\Path\\OIDC-KeyStorePath";
    conf.DisableKeyGeneration = false;
});
```

The account executing the server needs write permissions to the KeyStorePath, if its meant to generate the keys.