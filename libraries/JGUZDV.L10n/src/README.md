# JGUZDV.L10n

This module provides a default model for localizable (l10n) strings.  
It's built to be JSON convertible and also comes with a storage converter to use it in EntityFramework (see JGUZDV.L10n.EntityFramework).

The `.ToString()` method is overriden to use `CurrentCulture` to find the requested language and will always try to return anything.
There's also an explicit conversion `(string)` which uses the same mechanism.

