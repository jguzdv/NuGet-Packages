using JGUZDV.DynamicForms.Model;
using JGUZDV.DynamicForms.Samples.Client.Model;

using Microsoft.EntityFrameworkCore;

using Newtonsoft.Json;

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

        modelBuilder
            .Entity<Document>()
            .Property(x => x.Fields)
            .HasConversion(
                v => JsonConvert.SerializeObject(v, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All }),
                v => JsonConvert.DeserializeObject<List<Field>>(v, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All }) ?? new()
            );

        modelBuilder
           .Entity<DocumentDefinition>()
           .Property(x => x.FieldDefinitions)
           .HasConversion(
               v => JsonConvert.SerializeObject(v, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All }),
               v => JsonConvert.DeserializeObject<List<FieldDefinition>>(v, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All }) ?? new()
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
