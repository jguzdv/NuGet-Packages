using JGUZDV.JobHost.Database.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace JGUZDV.JobHost.Database
{
    /// <inheritdoc/>
    public class JobHostContext : DbContext
    {
        /// <inheritdoc/>
        public JobHostContext(DbContextOptions options) : base(options) { }

        /// <inheritdoc/>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // TODO: Do we want to assume relational database provider?
            modelBuilder.HasDefaultSchema(nameof(JobHost));

            modelBuilder.Entity<Job>()
                .HasOne(x => x.Host)
                .WithMany()
                .HasForeignKey(x => x.HostId);

            modelBuilder.Entity<Job>()
                .HasIndex(x => new { x.HostId, x.Name })
                .IsUnique();

            modelBuilder.Entity<Host>()
                .HasIndex(x => x.Name)
                .IsUnique();

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
        /// <summary>
        /// DbSet for the Jobs
        /// </summary>
        public virtual DbSet<Job> Jobs { get; set; }
        /// <summary>
        /// DbSet for the hosts
        /// </summary>
        public virtual DbSet<Entities.Host> Hosts { get; set; }
    }
}
