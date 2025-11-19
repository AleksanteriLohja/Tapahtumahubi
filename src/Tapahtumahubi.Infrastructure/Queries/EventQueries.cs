using System;
using System.Linq;
using Tapahtumahubi.Domain;

namespace Tapahtumahubi.Infrastructure;

public static class EventQueries
{
    /// <summary>
    /// Järjestää tulevat ensin ja sen sisällä StartTime nousevasti.
    /// </summary>
    public static IOrderedQueryable<Event> OrderUpcomingFirst(this IQueryable<Event> query, DateTime now)
        => query
            .OrderBy(e => e.StartTime < now) // false(0)=tulevat ensin, true(1)=menneet viimeiseksi
            .ThenBy(e => e.StartTime);

    /// <summary>
    /// Haku otsikosta, paikasta ja kuvauksesta (case-insensitive).
    /// </summary>
    public static IQueryable<Event> Search(this IQueryable<Event> query, string? term)
    {
        if (string.IsNullOrWhiteSpace(term)) return query;
        term = term.Trim().ToLowerInvariant();

        return query.Where(e =>
            ((e.Title ?? "").ToLower()).Contains(term) ||
            ((e.Location ?? "").ToLower()).Contains(term) ||
            ((e.Description ?? "").ToLower()).Contains(term));
    }
}