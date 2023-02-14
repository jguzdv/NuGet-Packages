# JGUZDV.L10n.EntityFramework

This project provides an extension method to configure a conversion of all `L10nString` instances to json.

```
public class MyDbContext : DbContext
{
    // ...    

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);

        configurationBuilder.ConfigureL10nStrings();
    }

    // ...
}
```