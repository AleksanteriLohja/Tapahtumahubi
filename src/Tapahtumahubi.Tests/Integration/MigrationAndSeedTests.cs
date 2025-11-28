using System.Linq;
using System.Threading.Tasks;
using Xunit;

using Tapahtumahubi.Infrastructure;
using Tapahtumahubi.Tests.Infrastructure; // <- FIX

namespace Tapahtumahubi.Tests.Integration;

public class MigrationAndSeedTests : IClassFixture<SqliteInMemoryFixture>
{
    private readonly SqliteInMemoryFixture _fx;

    public MigrationAndSeedTests(SqliteInMemoryFixture fx) => _fx = fx;

    [Fact]
    public async Task Can_migrate_and_seed_idempotently()
    {
        await using var ctx = _fx.CreateContext();

        await ctx.Database.EnsureDeletedAsync();
        await ctx.Database.EnsureCreatedAsync();

        await AppDbContextSeed.SeedAsync(ctx);
        var first = ctx.Events.Count();

        await AppDbContextSeed.SeedAsync(ctx);
        var second = ctx.Events.Count();

        Assert.True(first > 0);
        Assert.Equal(first, second);
    }
}
