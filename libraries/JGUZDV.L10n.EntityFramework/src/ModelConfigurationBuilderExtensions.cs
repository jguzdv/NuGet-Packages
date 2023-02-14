using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace JGUZDV.L10n.EntityFramework;

public static class ModelConfigurationBuilderExtensions
{
    public static void ConfigureL10nStrings(this ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<L10nString>()
            .HaveConversion<L10nConverter>();
        
        configurationBuilder.DefaultTypeMapping<L10nString>()
            .HasConversion<L10nConverter>();
    }
}
