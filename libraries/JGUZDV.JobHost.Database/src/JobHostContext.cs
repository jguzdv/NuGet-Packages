using JGUZDV.JobHost.Abstractions;
using JGUZDV.JobHost.Database.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Options;

namespace JGUZDV.JobHost.Database
{
    /// <inheritdoc/>
    public class JobHostContext : DbContext, IJobExecutionReporter
    {
        private readonly IOptions<JobReportOptions> _jobReportOptions;

        /// <inheritdoc/>
        public JobHostContext(DbContextOptions options, IOptions<JobReportOptions> jobReportOptions) : base(options)
        {
            _jobReportOptions = jobReportOptions;
        }

        /// <inheritdoc/>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // TODO: Do we want to assume relational database provider?
            modelBuilder.HasDefaultSchema(nameof(JobHost));

            modelBuilder.Entity<Entities.Job>()
                .HasOne(x => x.Host)
                .WithMany()
                .HasForeignKey(x => x.HostId);

            modelBuilder.Entity<Entities.Job>()
                .HasIndex(x => new { x.HostId, x.Name })
                .IsUnique();

            modelBuilder.Entity<Entities.Host>()
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
        public virtual DbSet<Host> Hosts { get; set; }

        public async Task RegisterHostAndJobsAsync(JobHostDescription jobHost)
        {
            var host = await Hosts.FirstOrDefaultAsync(x => x.Name == jobHost.HostName);

            if (host == null)
            {
                // register the host
                host = new Database.Entities.Host
                {
                    MonitoringUrl = jobHost.MonitoringUrl,
                    Name = jobHost.HostName
                };

                Hosts.Add(host);
                await SaveChangesAsync();
            }
            else
            {
                // clean up old jobs
                var jobs = await Jobs
                    .AsNoTracking()
                    .Where(x => x.HostId == host.Id)
                    .ToListAsync();

                var oldJobs = jobs
                    .Where(x => !jobHost.Jobs.Any(y => y.Name == x.Name))
                    .Select(x => x.Id)
                    .ToList();

                await Jobs
                    .Where(x => oldJobs.Contains(x.Id))
                    .ExecuteDeleteAsync();

                await SaveChangesAsync();
            }

            // register jobs
            foreach (var item in jobHost.Jobs)
            {
                var job = await Jobs.FirstOrDefaultAsync(x => x.HostId == host.Id && x.Name == item.Name);

                if (job == null)
                {
                    job = new Database.Entities.Job
                    {
                        LastExecutedAt = DateTimeOffset.MinValue,
                        NextExecutionAt = item.NextExecutionAt,
                        LastResult = "",
                        Schedule = item.CronSchedule,
                        Name = item.Name,
                        HostId = host.Id,
                    };

                    Jobs.Add(job);
                }
            }

            await SaveChangesAsync();
        }

        public async Task ReportJobExecutionAsync(JobExecutionReport jobReport)
        {
            var name = jobReport.Name;
            var host = jobReport.Host;

            var job = await Jobs.FirstAsync(x => x.Name == name && x.Host!.Name == host);

            if (!jobReport.Failed)
            {
                job.LastResult = "success";
            }
            else
            {
                job.LastResult = "error";
                job.FailMessage = jobReport.FailMessage;
            }

            job.RunTime = jobReport.RunTime;
            job.NextExecutionAt = jobReport.NextFireTimeUtc;
            job.LastExecutedAt = jobReport.FireTimeUtc;
            job.ShouldExecute = false;

            await SaveChangesAsync();
        }

        public async Task<List<Abstractions.Model.Job>> GetPendingJobs()
        {
            var host = _jobReportOptions.Value.JobHostName;
            var jobs = await Jobs
                .Where(x => x.Host.Name == host && x.ShouldExecute == true)
                .Select(x => new Abstractions.Model.Job
                {
                    FailMessage = x.FailMessage,
                    HostId = x.HostId,
                    Id = x.Id,
                    LastExecutedAt = x.LastExecutedAt,
                    LastResult = x.LastResult,
                    Name = x.Name,
                    NextExecutionAt = x.NextExecutionAt,
                    RunTime = x.RunTime,
                    Schedule = x.Schedule,
                    ShouldExecute = x.ShouldExecute
                })
                .ToListAsync();

            return jobs;
        }

        public async Task RemoveFromPending(int jobId)
        {
            var entity = await Jobs.FirstAsync(x => x.Id == jobId && x.Host.Name == _jobReportOptions.Value.JobHostName);
            entity.ShouldExecute = false;
            await SaveChangesAsync();
        }
    }
}
