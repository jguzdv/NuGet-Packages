using System.Text.Json;

using JGUZDV.DynamicForms.Model;
using JGUZDV.DynamicForms.Samples.Client.Model;

using Microsoft.EntityFrameworkCore;


namespace JGUZDV.DynamicForms.Samples.DataAccess;
/// <summary>
/// Test db context connecting to local db
/// </summary>
public class TestDbContext : DbContext
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="options"></param>
    public TestDbContext(DbContextOptions options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var options = new JsonSerializerOptions();
        modelBuilder
            .Entity<Document>()
            .Property(x => x.Fields)
            .HasConversion(
                v => JsonSerializer.Serialize(v, options),
                v => JsonSerializer.Deserialize<List<Field>>(v, options) ?? new()
            );

        modelBuilder
           .Entity<DocumentDefinition>()
           .Property(x => x.FieldDefinitions)
           .HasConversion(
                v => JsonSerializer.Serialize(v, options),
                v => JsonSerializer.Deserialize<List<FieldDefinition>>(v, options) ?? new()
           );
    }

    /// <summary>
    /// 
    /// </summary>
    public DbSet<Document> Documents { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public DbSet<DocumentDefinition> DocumentDefinitions { get; set; }
}
