using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Debug;

namespace JGUZDV.L10n.EntityFramework.Tests;

public class DbContextFixture
{
    public TestDbContext TestDb { get; set; }

    public DbContextFixture()
    {
        var connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=Integration_Test_#Token#;Trusted_Connection=True;Encrypt=False".Replace("#Token#", Guid.NewGuid().ToString());
        var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseLoggerFactory(new LoggerFactory(new[] { new DebugLoggerProvider() }))
                .UseSqlServer(connectionString)
                .Options;
        TestDb = new(options);

        TestDb.Database.EnsureCreated();
    }

    public void Dispose()
    {
        TestDb.Database.EnsureDeleted();
    }
}
