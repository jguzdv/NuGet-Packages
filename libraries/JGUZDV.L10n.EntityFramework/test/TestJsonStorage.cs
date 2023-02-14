using System.Text.Json;

using Microsoft.EntityFrameworkCore;

namespace JGUZDV.L10n.EntityFramework.Tests;

public class TestJsonStorage : IClassFixture<DbContextFixture>
{
    private readonly DbContextFixture _fixture;

    public TestJsonStorage(DbContextFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task TestConfiguration()
    {
        _fixture.TestDb.Blogs.Add(new()
        {
            Author = "test",
            Title = new(new Dictionary<string, string> { { "de", "Deutsch" }, { "en", "English" } })
        });

        await _fixture.TestDb.SaveChangesAsync();

        var content = await _fixture.TestDb.Database
            .SqlQueryRaw<string>("SELECT Title as Value FROM Blogs")
            .ToListAsync();

        Assert.Single(content);

        var parsedContent = JsonSerializer.Deserialize<L10nString>(content[0]);

        Assert.NotNull(parsedContent);
        Assert.Equal("Deutsch", parsedContent["de"]);
        Assert.Equal("English", parsedContent["en"]);
    }
}