using System;
using System.Linq;
using System.Collections.Generic;
using Tapahtumahubi.Domain;

namespace Tapahtumahubi.Infrastructure;

public static class AppDbContextSeed
{
    /// <summary>
    /// Lisää demotapahtumat vain, jos Events-taulu on tyhjä.
    /// Tarkoitettu DEBUG-käyttöön.
    /// </summary>
    public static void Seed(AppDbContext db)
    {
        if (db.Events.Any())
            return;

        var now = DateTime.Now;

        var demo = new List<Event>
        {
            new Event
            {
                Title = "Demo: Kehittäjäpäivä",
                Location = "Helsinki",
                StartTime = now.AddDays(1).Date.AddHours(10),
                Description = "Esimerkkitapahtuma – luotu seedistä.",
                MaxParticipants = 50
            },
            new Event
            {
                Title = "Demo: Tietoturva-workshop",
                Location = "Turku",
                StartTime = now.AddDays(3).Date.AddHours(13),
                Description = "Käytännön harjoituksia ja esityksiä.",
                MaxParticipants = 30
            },
            new Event
            {
                Title = "Demo: Retrospektiivi",
                Location = "Kuopio",
                StartTime = now.AddDays(-1).Date.AddHours(15), // mennyt → testaa järjestystä
                Description = "Mennyt tapahtuma – testaa järjestystä.",
                MaxParticipants = 20
            }
        };

        db.Events.AddRange(demo);
        db.SaveChanges();
    }
}
