# JGUZDV.Extensions.Authorization

ClaimRequirements are meant to be a simple, serializable way to configure one or multiple required claims to be authorized.
There's also a package containing a [blazor component for configuring the requirements via UI](https://www.nuget.org/packages/JGUZDV.Blazor.Components.ClaimRequirementEditor/)

Essentially the package provides two classes, which both are inheritors of `ClaimRequirement` and implement `IsSatisfiedBy(ClaimsPrincipal principal)`.
The Method will check, if the given principal satisfies the requirement and return true or false respectively.

It supports requirements for a single claim value (via `ClaimValueRequirement`) and multiple values (via `ClaimRequirementCollection`).
Multiple values can be checked with `MatchAny` or `MatchAll` allowing OR and AND logic, respectively.

The package is meant to be used in frontends and backends alike.
You can easily json serialize the `ClaimRequirement` and store it in a database or send it over the wire.

**EF Value conversion**
```csharp
public class MyDbContext : DbContext
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<MyClass>()
            .Property(x => x.ClaimRequirement)
            .HasConversion(new ValueConverter<ClaimRequirement, string>(
                (req) => JsonSerializer.Serialize(req, JsonSerializerOptions.Default),
                (reqJson) => JsonSerializer.Deserialize<ClaimRequirement>(reqJson, JsonSerializerOptions.Default) ?? new NullRequirement()
                ));
    }
}
```

There's also a class (`ClaimRequirementOptions`), so it can be used in appsettings.json:
**appsettings.json**
```json
// appsettings.json
{
    "ClaimRequirementCollection": {
        "MatchType": "MatchAny",
        "Requirements": [
            {
                "ClaimType": "c1",
                "ClaimValue": "v1"
            },
            {
                "ClaimType": "c2",
                "ClaimValue": "v2"
            }
        ]
    },

    "ClaimValueRequirement": {
        "ClaimType": "c1",
        "ClaimValue": "v1"
    }
}
```
