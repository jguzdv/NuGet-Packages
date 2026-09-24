using JGUZDV.Outbox.EntityFrameworkCore;

namespace Microsoft.EntityFrameworkCore;

/// <summary>
/// Provides extension methods for the DbModelBuilder class to configure the outbox message entity in the Entity Framework model.
/// </summary>
public static class DbModelBuilderExtensions
{
    /// <summary>
    /// Configures the DbModelBuilder to include the OutboxMessage entity and its related configurations in the Entity Framework model.
    /// </summary>
    public static ModelBuilder UseOutbox(this ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new OutboxMessageEntityTypeConfiguration());
        
        return modelBuilder;
    }
}