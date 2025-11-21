using System;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Tapahtumahubi.Infrastructure;
using Xunit;

namespace Tapahtumahubi.Tests.Infrastructure;

public sealed class SqliteInMemoryFixture : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly DbContextOptions<AppDbContext> _options;

    public SqliteInMemoryFixture()
    {
        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();

        _options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;

        // Luo skeema nykyisestä mallista (ei migraatioita, nopea integraatiotesti)
        using var db = new AppDbContext(_options);
        db.Database.EnsureCreated();
    }

    public AppDbContext CreateContext() => new AppDbContext(_options);

    public void Dispose()
    {
        _connection.Dispose();
    }
}

// xUnit-kokoelma, jotta fixtuuri jaetaan testeille
[CollectionDefinition("sqlite")]
public class SqliteCollection : ICollectionFixture<SqliteInMemoryFixture> { }