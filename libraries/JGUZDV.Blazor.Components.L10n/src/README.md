# JGUZDV.Blazor.Components.L10n

This module allows the easy use of an edit form for L10n strings. 
For this purpose, the component initially accepts an L10n string `L10n` and an event callback of an L10n string `L10nChanged`.

**Page.razor**
```html
<JGUZDV.Blazor.Components.L10n.L10nEditor L10n="_firstTestVaue" L10nChanged="ShowChangedValue"></JGUZDV.Blazor.Components.L10n.L10nEditor>
```

There are two options for integrating the supported languages. 
These can be passed as parameters `SupportedCultures` to the component. 

**Page.razor**
```html
<JGUZDV.Blazor.Components.L10n.L10nEditor L10n="_secondRestValue" SupportedCultures="_languages" L10nChanged="ShowChangedValue"></JGUZDV.Blazor.Components.L10n.L10nEditor>
```

The other variant offers the option of specifying the supporting languages ​​once in the Program.cs.

**Program.cs**
```csharp
builder.Services.AddSupportedCultures(new List<string>() { "de", "en" });
```

It is possible to use both variants and only pass the supported languages ​​as parameters if they differ from the standard supported languages.
