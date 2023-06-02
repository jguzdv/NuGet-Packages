# JGUZDV.Extensions.Authorization

ClaimRequirements are meant to be a simple, serializable way to configure one or multiple required claims to be authorized.

Esentially there are two types at play `ClaimValueRequirement` and `ClaimRequirementCollection` both are inheritors of `ClaimRequirement` and both will implement `SatisfiesRequirement(ClaimsPrincipal principal)`.

You can save them as json or use them from a config file:

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

```csharp
public class OptionsDTO
{
    public ClaimRequirementOptions ClaimRequirementCollection { get; set; }
    public ClaimRequirementOptions ClaimValueRequirement { get; set; }
}
```

If you want them to be saved to a database, you'll need to provide a simple EF Value Converter, to map to and from JSON.