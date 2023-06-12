# JGUZDV.L10n

This module provides a default model for localizable (l10n) strings.  
It's built to be JSON convertible and also comes with a storage converter to use it in EntityFramework (see JGUZDV.L10n.EntityFramework).

The `.ToString()` method is overriden to use `CurrentCulture` to find the requested language and will always try to return anything.
There's also an explicit conversion `(string)` which uses the same mechanism.

```csharp
var l10n = new L10nString();
l10n["de"] = "Hallo Welt";
l10n["en"] = "Hello World";

var asString = (string)l10n;
```