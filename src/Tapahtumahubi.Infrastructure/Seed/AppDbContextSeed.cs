using System;
using System.Linq;
using System.Threading.Tasks;
using Tapahtumahubi.Domain;

namespace Tapahtumahubi.Infrastructure;

public static class AppDbContextSeed
{
    /// <summary>
    /// Idempotentti alustus. Turvallista ajaa useita kertoja.
    /// </summary>
    public static async Task SeedAsync(AppDbContext db)
    {
        // Luodaan tietokanta, jos puuttuu (SQLite in-memory persistent -yhteys huomioitu)
        await db.Database.EnsureCreatedAsync();

        // Esimerkkitapahtumat vain jos kantaan ei ole vielä seedattu.
        if (!db.Events.Any())
        {
            var now = DateTime.UtcNow;

            db.Events.AddRange(
                new Event
                {
                    Title = "Tiimikokous",
                    Location = "Teams",
                    StartTime = now.AddDays(1),
                    Description = "Sprintin tilannekatsaus",
                    MaxParticipants = 20
                },
                new Event
                {
                    Title = "Koodikatselmointi",
                    Location = "Toimisto",
                    StartTime = now.AddDays(2),
                    Description = "Domain + Infrastructure",
                    MaxParticipants = 10
                }
            );

            await db.SaveChangesAsync();
        }
    }
}