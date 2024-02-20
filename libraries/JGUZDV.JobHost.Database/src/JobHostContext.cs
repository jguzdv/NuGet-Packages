using JGUZDV.JobHost.Database.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace JGUZDV.JobHost.Database
{
    public class JobHostContext : DbContext
    {
        public JobHostContext(DbContextOptions options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // TODO: Do we want to assume relational database provider?
            modelBuilder.HasDefaultSchema(nameof(JobHost));

            modelBuilder.Entity<Job>()
                .HasOne(x => x.Host)
                .WithMany()
                .HasForeignKey(x => x.HostId);

            var converter = new ValueConverter<DateTimeOffset, DateTimeOffset>(x => x.ToOffset(TimeSpan.Zero), x => x.ToLocalTime());
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entity.GetProperties())
                {
                    if (property.ClrType == typeof(DateTimeOffset) || property.ClrType == typeof(DateTimeOffset?))
                    {

                        modelBuilder.Entity(entity.Name).Property(property.Name)
                            .HasConversion(converter);
                    }
                }
            }
        }

        public virtual DbSet<Job> Jobs { get; set; }
        public virtual DbSet<Entities.Host> Hosts { get; set; }
    }
}
