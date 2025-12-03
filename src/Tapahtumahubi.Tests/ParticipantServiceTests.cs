using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Tapahtumahubi.Domain;
using Tapahtumahubi.Infrastructure;
using Xunit;

namespace Tapahtumahubi.Tests.Infrastructure
{
    public sealed class ParticipantServiceTests : IDisposable
    {
        private readonly SqliteConnection _conn;
        private readonly DbContextOptions<AppDbContext> _opts;
        private readonly IDbContextFactory<AppDbContext> _factory;
        private readonly ParticipantService _svc;

        public ParticipantServiceTests()
        {
            // In-memory SQLite, yksi jaettu yhteys testiluokalle
            _conn = new SqliteConnection("DataSource=:memory:");
            _conn.Open();

            _opts = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(_conn)
                .EnableSensitiveDataLogging()
                .Options;

            // Luodaan skeema kerran
            using (var ctx = new AppDbContext(_opts))
            {
                ctx.Database.EnsureCreated();
            }

            _factory = new PseudoFactory(_opts);
            _svc = new ParticipantService(_factory);
        }

        [Fact]
        public async Task Add_and_List_by_Event_works()
        {
            var eventId = await CreateEventAsync("Test Event", max: 5);

            var p = await _svc.AddAsync(eventId, "John Doe", "john@example.com");
            Assert.True(p.Id > 0);

            var list = await _svc.ListByEventAsync(eventId);
            Assert.Single(list);
            Assert.Equal("John Doe", list[0].Name);
            Assert.Equal("john@example.com", list[0].Email);
        }

        [Fact]
        public async Task Duplicate_email_per_event_is_blocked()
        {
            var eventId = await CreateEventAsync("Test Event", max: 5);

            await _svc.AddAsync(eventId, "A", "a@x.com");
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _svc.AddAsync(eventId, "B", "a@x.com")
            );

            Assert.Contains("Sähköposti on jo ilmoitettu", ex.Message);
        }

        [Fact]
        public async Task Capacity_limit_is_enforced()
        {
            var eventId = await CreateEventAsync("Limited", max: 1);

            await _svc.AddAsync(eventId, "A", "a@x.com");
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _svc.AddAsync(eventId, "B", "b@x.com")
            );

            Assert.Contains("täynnä", ex.Message, StringComparison.OrdinalIgnoreCase);
        }

        // -------- apurit --------

        // Luo minimivaatimukset täyttävä Event-rivi ja palauta Id
        private async Task<int> CreateEventAsync(string title = "Test Event", int max = 10)
        {
            using var ctx = await _factory.CreateDbContextAsync();

            var e = new Event
            {
                Title = title,
                Location = "Test",
                Description = "",
                StartTime = DateTime.Today.AddHours(12),
                MaxParticipants = max
            };

            if (!e.Validate(out var errors))
                throw new InvalidOperationException(string.Join("; ", errors));

            ctx.Events.Add(e);
            await ctx.SaveChangesAsync();
            return e.Id;
        }

        public void Dispose()
        {
            _conn.Dispose();
        }

        // Pieni tehdas, jota käytetään testissä IDbContextFactoryna
        private sealed class PseudoFactory : IDbContextFactory<AppDbContext>
        {
            private readonly DbContextOptions<AppDbContext> _options;
            public PseudoFactory(DbContextOptions<AppDbContext> options) => _options = options;

            public AppDbContext CreateDbContext() => new AppDbContext(_options);

            // EF Core 8:ssa interface sisältää myös async-version; tarjotaan sekin.
            public Task<AppDbContext> CreateDbContextAsync(CancellationToken cancellationToken = default)
                => Task.FromResult(CreateDbContext());
        }
    }
}