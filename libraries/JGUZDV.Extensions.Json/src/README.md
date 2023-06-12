# JGUZDV.Extensions.Json

Provides DateOnly and TimeOnly converters for .NET6.0.
Provides StringTrimmingJsonConverter for converting strings to json while trimming them and setting them `null`, if empty.

**Program.cs** (Minimal API)
```csharp
builder.Services.Configure<JsonOptions>(x => {
    x.SerializerOptions.Converters.Add(new JsonStringTrimmingConverter());
    x.SerializerOptions.Converters.Add(new DateOnlyConverter());
    x.SerializerOptions.Converters.Add(new TimeOnlyConverter());
});
```

**Program.cs (defaults)**
```csharp
// This will also configure property name handling, key names and null handling.
builder.Services.Configure<JsonOptions>(x => x.SerializerOptions.SetJGUZDVDefaults());
```