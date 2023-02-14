using Microsoft.EntityFrameworkCore;

namespace JGUZDV.L10n.EntityFramework.Tests;

public class Blog
{
    public int Id { get; set; }

    public string? Author { get; set; }

    public L10nString? Title { get; set; }
}

public class TestDbContext : DbContext
{
    public TestDbContext(DbContextOptions<TestDbContext> options)
        :base(options)
    {

    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);

        configurationBuilder.ConfigureL10nStrings();
    }

    public DbSet<Blog> Blogs { get; set; }
}
