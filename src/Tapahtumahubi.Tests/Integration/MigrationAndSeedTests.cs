using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Tapahtumahubi.Domain;
using Tapahtumahubi.Infrastructure;
using Xunit;

namespace Tapahtumahubi.Tests.Integration
{
    public class MigrationAndSeedTests
    {
        private static DbContextOptions<AppDbContext> CreateOptions(SqliteConnection connection) =>
            new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(connection)
                .EnableSensitiveDataLogging()
                .Options;

        private static SqliteConnection CreatePersistentInMemoryConnection()
        {
            var conn = new SqliteConnection("DataSource=:memory:;Cache=Shared");
            conn.Open();
            return conn;
        }

        /// <summary>
        /// Kutsuu AppDbContextSeed.SeedAsync(ctx) tai AppDbContextSeed.Seed(ctx) – kumpi löytyy.
        /// Heijastus poistaa riippuvuuden tarkasta metodin nimestä.
        /// </summary>
        private static async Task InvokeSeederAsync(AppDbContext ctx)
        {
            var seedType = typeof(AppDbContextSeed);

            // 1) Yritä async-metodia
            var mAsync = seedType.GetMethod(
                "SeedAsync",
                BindingFlags.Public | BindingFlags.Static,
                binder: null,
                types: new[] { typeof(AppDbContext) },
                modifiers: null);

            if (mAsync is not null)
            {
                var task = (Task?)mAsync.Invoke(null, new object[] { ctx });
                if (task is not null) await task;
                return;
            }

            // 2) Yritä synkronista Seed(ctx)
            var mSync = seedType.GetMethod(
                "Seed",
                BindingFlags.Public | BindingFlags.Static,
                binder: null,
                types: new[] { typeof(AppDbContext) },
                modifiers: null);

            if (mSync is not null)
            {
                mSync.Invoke(null, new object[] { ctx });
                return;
            }

            throw new InvalidOperationException(
                "AppDbContextSeed-luokasta ei löytynyt SeedAsync(AppDbContext) eikä Seed(AppDbContext).");
        }

        [Fact]
        public async Task Migrations_apply_and_seed_is_idempotent()
        {
            using var conn = CreatePersistentInMemoryConnection();
            var options = CreateOptions(conn);

            // 1) Migraatiot + ensimmäinen siemen
            await using (var ctx = new AppDbContext(options))
            {
                await ctx.Database.MigrateAsync();
                await InvokeSeederAsync(ctx);
            }

            int eventsAfterFirstSeed, participantsAfterFirstSeed;

            await using (var verify1 = new AppDbContext(options))
            {
                eventsAfterFirstSeed = await verify1.Events.CountAsync();
                participantsAfterFirstSeed = await verify1.Participants.CountAsync();
                Assert.True(eventsAfterFirstSeed >= 0);
                Assert.True(participantsAfterFirstSeed >= 0);
            }

            // 2) Uusintasiemen – ei duplikaatteja
            await using (var ctx2 = new AppDbContext(options))
            {
                await InvokeSeederAsync(ctx2);
            }

            await using (var verify2 = new AppDbContext(options))
            {
                var eventsAfterSecondSeed = await verify2.Events.CountAsync();
                var participantsAfterSecondSeed = await verify2.Participants.CountAsync();

                Assert.Equal(eventsAfterFirstSeed, eventsAfterSecondSeed);
                Assert.Equal(participantsAfterFirstSeed, participantsAfterSecondSeed);
            }
        }

        [Fact]
        public async Task Unique_index_blocks_duplicate_email_in_same_event()
        {
            using var conn = CreatePersistentInMemoryConnection();
            var options = CreateOptions(conn);

            await using var ctx = new AppDbContext(options);
            await ctx.Database.MigrateAsync();

            var ev = new Event
            {
                Title = "Testievent",
                StartTime = DateTime.UtcNow.AddDays(1),
                Location = "Testipaikka"
            };

            ctx.Events.Add(ev);
            await ctx.SaveChangesAsync();

            ctx.Participants.Add(new Participant { EventId = ev.Id, Name = "Anna", Email = "demo@example.com" });
            await ctx.SaveChangesAsync();

            ctx.Participants.Add(new Participant { EventId = ev.Id, Name = "Bertta", Email = "demo@example.com" });

            await Assert.ThrowsAsync<DbUpdateException>(() => ctx.SaveChangesAsync());
        }
    }
}