using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

using Tapahtumahubi.Domain;
using Tapahtumahubi.Infrastructure;

namespace Tapahtumahubi.Tests
{
    public sealed class ParticipantServiceTests : IDisposable
    {
        private readonly SqliteConnection _conn;
        private readonly DbContextOptions<AppDbContext> _opts;
        private readonly AppDbContext _ctx;
        private readonly ParticipantService _svc;

        public ParticipantServiceTests()
        {
            _conn = new SqliteConnection("DataSource=:memory:");
            _conn.Open();

            _opts = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(_conn)
                .Options;

            _ctx = new AppDbContext(_opts);
            _ctx.Database.EnsureCreated();

            // Jos teillä on seeder, voit kutsua sitä tässä (valinnainen):
            // await AppDbContextSeed.SeedAsync(_ctx);

            // Palvelun luonti:
            _svc = new ParticipantService(_ctx);
            // Jos teillä on ctor(IDbContextFactory<AppDbContext>):
            // _svc = new ParticipantService(new PseudoFactory(_opts));
        }

        public void Dispose()
        {
            _ctx.Dispose();
            _conn.Dispose();
        }

        // Luo minimivaatimukset täyttävä Event rivi ja palauta Id
        private async Task<int> CreateEventAsync(string label = "Test Event")
        {
            var e = new Event
            {
                // TODO: jos teillä on "Title" → käytä Title; jos "Name" → käytä Name.
                // Alla oletetaan "Title". Vaihda tarvittaessa.
                Title = label
            };
            _ctx.Add(e);
            await _ctx.SaveChangesAsync();
            return e.Id;
        }

        private static Participant P(string name, string email) =>
            new Participant { Name = name, Email = email };

        [Fact]
        public async Task AddAsync_Persists_And_Returns_Id()
        {
            var eventId = await CreateEventAsync();
            var created = await _svc.AddAsync(eventId, "Alice", "alice@example.test");

            Assert.True(created.Id > 0);
            var fromDb = await _ctx.Participants.SingleAsync(x => x.Id == created.Id);
            Assert.Equal("Alice", fromDb.Name);
            Assert.Equal("alice@example.test", fromDb.Email);
            Assert.Equal(eventId, fromDb.EventId);
        }

        [Fact]
        public async Task ListByEventAsync_Filters_By_Event()
        {
            var e1 = await CreateEventAsync("E1");
            var e2 = await CreateEventAsync("E2");

            await _svc.AddAsync(e1, "Eve", "eve@ex.test");
            await _svc.AddAsync(e2, "Frank", "frank@ex.test");

            var list1 = await _svc.ListByEventAsync(e1);
            Assert.All(list1, p => Assert.Equal(e1, p.EventId));
            Assert.Contains(list1, p => p.Email == "eve@ex.test");
            Assert.DoesNotContain(list1, p => p.Email == "frank@ex.test");
        }

        [Fact]
        public async Task UpdateAsync_Changes_Are_Saved()
        {
            var eventId = await CreateEventAsync();
            var created = await _svc.AddAsync(eventId, "Carol", "carol@ex.test");

            await _svc.UpdateAsync(created.Id, "Carol Updated", "carol.updated@ex.test");

            var fromDb = await _ctx.Participants.SingleAsync(x => x.Id == created.Id);
            Assert.Equal("Carol Updated", fromDb.Name);
            Assert.Equal("carol.updated@ex.test", fromDb.Email);
        }

        [Fact]
        public async Task DeleteAsync_Removes_Row()
        {
            var eventId = await CreateEventAsync();
            var created = await _svc.AddAsync(eventId, "Dave", "dave@ex.test");

            await _svc.DeleteAsync(created.Id);

            var exists = await _ctx.Participants.AnyAsync(x => x.Id == created.Id);
            Assert.False(exists);
        }

        // Jos ctor vaatii IDbContextFactory<AppDbContext>, käytä tätä pientä apuluokkaa:
        private sealed class PseudoFactory : IDbContextFactory<AppDbContext>
        {
            private readonly DbContextOptions<AppDbContext> _o;
            public PseudoFactory(DbContextOptions<AppDbContext> o) => _o = o;
            public AppDbContext CreateDbContext() => new AppDbContext(_o);
        }
    }
}