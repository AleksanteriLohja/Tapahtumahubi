using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Tapahtumahubi.Domain;
using Tapahtumahubi.Infrastructure;
using Xunit;

namespace Tapahtumahubi.Tests.Infrastructure
{
    public class EventQueriesTests
    {
        private static IQueryable<Event> Seed(DateTime now)
        {
            // Huom: Id:t helpottavat järjestyksen tarkastusta
            return new List<Event>
            {
                new() { Id = 1, Title = "Vanha tapahtuma",      StartTime = now.AddDays(-2), Location = "Turku",   Description = "historia" },
                new() { Id = 2, Title = "Äskettäin mennyt",      StartTime = now.AddHours(-1), Location = "Kuopio", Description = "mennyt" },
                new() { Id = 3, Title = "Eilen",                 StartTime = now.AddDays(-1), Location = "Oulu",    Description = "retro" },
                new() { Id = 4, Title = "Yö Gala",               StartTime = now,             Location = "Helsinki",Description = "Rock" },
                new() { Id = 5, Title = "CONCERT",               StartTime = now.AddMinutes(1), Location = "Espoo", Description = "kulttuuri" },
                new() { Id = 6, Title = "Brunch",                StartTime = now.AddDays(1),   Location = "YÖBAARI",Description = "ruoka" }
            }.AsQueryable();
        }

        [Fact]
        public void OrderUpcomingFirst_UpcomingThenPast_WithBoundaryNow()
        {
            var now = new DateTime(2025, 11, 26, 10, 0, 0, DateTimeKind.Unspecified);
            var data = Seed(now);

            var ordered = data.OrderUpcomingFirst(now).Select(e => e.Id).ToList();

            // Tulevat (mukaan lukien StartTime == now) nousevasti: 4(now),5(+1min),6(+1d)
            // Menneet nousevasti: 1(-2d),3(-1d),2(-1h)
            Assert.Equal(new[] { 4, 5, 6, 1, 3, 2 }, ordered);
        }

        [Fact]
        public void Search_NullOrWhitespace_ReturnsOriginal()
        {
            var now = DateTime.Now;
            var data = Seed(now);

            Assert.Equal(data.Count(), data.Search(null).Count());
            Assert.Equal(data.Count(), data.Search("").Count());
            Assert.Equal(data.Count(), data.Search("   ").Count());
        }

        [Theory]
        [InlineData("yö", new[] { 4, 6 })]          // Title "Yö Gala", Location "YÖBAARI"
        [InlineData("CONCERT", new[] { 5 })]        // Title case-insensitive
        [InlineData("rock", new[] { 4 })]           // Description
        [InlineData("oulu", new[] { 3 })]           // Location
        public void Search_MatchesTitleLocationDescription_CaseInsensitive(string term, int[] expectedIds)
        {
            var now = DateTime.Now;
            var data = Seed(now);

            var result = data.Search(term).Select(e => e.Id).OrderBy(x => x).ToArray();

            Assert.Equal(expectedIds.OrderBy(x => x), result);
        }

        [Theory]
        [InlineData("fi-FI")]
        [InlineData("en-US")]
        public void Search_WorksUnderDifferentCultures(string culture)
        {
            var now = DateTime.Now;
            var data = Seed(now);

            using var _ = new CultureScope(culture);
            // Tarkistetaan ääkkösten case-insensitive –haku
            var res = data.Search("YÖ").Select(e => e.Id).OrderBy(i => i).ToArray();

            Assert.Equal(new[] { 4, 6 }, res);
        }

        [Fact]
        public void Search_Then_OrderUpcomingFirst_ComposesCorrectly()
        {
            var now = new DateTime(2025, 11, 26, 10, 0, 0);
            var data = Seed(now);

            var ids = data
                .Search("o")              // osuu moneen (Oulu, Concert, Yö Gala, etc.)
                .OrderUpcomingFirst(now)  // ensin tulevat + now
                .Select(e => e.Id)
                .ToList();

            // Joukossa oltava tulevat ensin (4,5,6) tämän haun filttereillä;
            // ei kuitenkaan tarkka joukko, koska "o" osuu moneen – tarkistetaan relative order.
            var firstPastIndex = ids.FindIndex(id => id is 1 or 2 or 3);
            Assert.True(firstPastIndex >= 0, "Past-eventti tulee jossain kohtaa.");
            foreach (var futureId in new[] { 4, 5, 6 })
            {
                var ix = ids.IndexOf(futureId);
                if (ix >= 0) Assert.True(ix < firstPastIndex, "Tulevien pitää sijaita ennen menneitä.");
            }
        }
    }

    /// <summary>
    /// Vaihtaa väliaikaisesti CurrentCulture & CurrentUICulture testin ajaksi.
    /// </summary>
    internal sealed class CultureScope : IDisposable
    {
        private readonly CultureInfo _prevCulture = CultureInfo.CurrentCulture;
        private readonly CultureInfo _prevUi = CultureInfo.CurrentUICulture;

        public CultureScope(string name)
        {
            var ci = new CultureInfo(name);
            CultureInfo.CurrentCulture = ci;
            CultureInfo.CurrentUICulture = ci;
        }

        public void Dispose()
        {
            CultureInfo.CurrentCulture = _prevCulture;
            CultureInfo.CurrentUICulture = _prevUi;
        }
    }
}