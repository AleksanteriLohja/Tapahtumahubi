using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Tapahtumahubi.Domain;
using Tapahtumahubi.Infrastructure;
using Tapahtumahubi.Tests.Infrastructure;
using Xunit;

namespace Tapahtumahubi.Tests.Integration;

[Collection("sqlite")]
public class EventIntegrationTests
{
    private readonly SqliteInMemoryFixture _fx;

    public EventIntegrationTests(SqliteInMemoryFixture fx) => _fx = fx;

    [Fact(DisplayName = "Event: Create → Read → Update toimii")]
    public async Task CreateReadUpdate_Succeeds()
    {
        // CREATE
        using (var db = _fx.CreateContext())
        {
            var ev = new Event
            {
                Title = "Testi-Event",
                Location = "Helsinki",
                StartTime = DateTime.Today.AddDays(1).AddHours(9),
                Description = "Kuvaus"
                // MaxParticipants jätetään testaamaan mahdollisesti defaultia eri testissä
            };
            db.Events.Add(ev);
            await db.SaveChangesAsync();
        }

        // READ + UPDATE
        using (var db = _fx.CreateContext())
        {
            var created = await db.Events.FirstOrDefaultAsync(e => e.Title == "Testi-Event");
            Assert.NotNull(created);

            created!.Title = "Päivitetty";
            await db.SaveChangesAsync();
        }

        // VERIFY
        using (var db = _fx.CreateContext())
        {
            var updated = await db.Events.FirstOrDefaultAsync(e => e.Title == "Päivitetty");
            Assert.NotNull(updated);
            Assert.Equal("Helsinki", updated!.Location);
        }
    }

    [Fact(DisplayName = "Event delete cascade poistaa myös osallistujat")]
    public async Task DeleteEvent_CascadesToParticipants()
    {
        int eventId;

        // Luo event + osallistujat
        using (var db = _fx.CreateContext())
        {
            var ev = new Event
            {
                Title = "Cascade",
                Location = "Turku",
                StartTime = DateTime.Today.AddDays(2).AddHours(10),
                MaxParticipants = 100
            };
            db.Events.Add(ev);
            await db.SaveChangesAsync();

            eventId = ev.Id;

            db.Participants.AddRange(
                new Participant { Name = "A", Email = "a@test.fi", EventId = eventId },
                new Participant { Name = "B", Email = "b@test.fi", EventId = eventId }
            );
            await db.SaveChangesAsync();
        }

        // Varmista että osallistujat tulivat
        using (var db = _fx.CreateContext())
        {
            var count = await db.Participants.CountAsync(p => p.EventId == eventId);
            Assert.Equal(2, count);
        }

        // Poista event
        using (var db = _fx.CreateContext())
        {
            var ev = await db.Events.FirstAsync(e => e.Id == eventId);
            db.Events.Remove(ev);
            await db.SaveChangesAsync();
        }

        // Cascade: osallistujat poissa
        using (var db = _fx.CreateContext())
        {
            var left = await db.Participants.CountAsync(p => p.EventId == eventId);
            Assert.Equal(0, left);
        }
    }

    [Fact(DisplayName = "MaxParticipants default arvo on 50 kun ei aseteta")]
    public async Task Default_MaxParticipants_Is_50()
    {
        int id;
        using (var db = _fx.CreateContext())
        {
            var ev = new Event
            {
                Title = "DefaultMax",
                Location = "Kuopio",
                StartTime = DateTime.Today.AddDays(3).AddHours(12),
                // MaxParticipants jätetään tarkoituksella asettamatta
            };
            db.Events.Add(ev);
            await db.SaveChangesAsync();
            id = ev.Id;
        }

        // Lue ja varmista että default 50 on tullut kannasta
        using (var db = _fx.CreateContext())
        {
            var reloaded = await db.Events.FirstAsync(e => e.Id == id);
            Assert.Equal(50, reloaded.MaxParticipants);
        }
    }
}