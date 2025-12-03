using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Tapahtumahubi.Domain;

namespace Tapahtumahubi.Infrastructure
{
    public interface IParticipantService
    {
        Task<List<Participant>> ListByEventAsync(int eventId);
        Task<Participant> AddAsync(int eventId, string name, string email);
        Task UpdateAsync(int id, string name, string email);
        Task DeleteAsync(int id);
    }

    public class ParticipantService : IParticipantService
    {
        private readonly IDbContextFactory<AppDbContext> _factory;

        public ParticipantService(IDbContextFactory<AppDbContext> factory)
        {
            _factory = factory;
        }

        public async Task<List<Participant>> ListByEventAsync(int eventId)
        {
            using var db = await _factory.CreateDbContextAsync();
            return await db.Participants
                .AsNoTracking()
                .Where(p => p.EventId == eventId)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<Participant> AddAsync(int eventId, string name, string email)
        {
            using var db = await _factory.CreateDbContextAsync();

            var ev = await db.Events
                .Include(e => e.Participants)
                .SingleOrDefaultAsync(e => e.Id == eventId)
                ?? throw new InvalidOperationException("Tapahtumaa ei löytynyt.");

            if (ev.Participants.Count >= ev.MaxParticipants)
                throw new InvalidOperationException("Tapahtuma on täynnä.");

            var duplicate = await db.Participants
                .AnyAsync(x => x.EventId == eventId && x.Email == email);
            if (duplicate)
                throw new InvalidOperationException("Sähköposti on jo ilmoitettu tähän tapahtumaan.");

            var p = new Participant { EventId = eventId, Name = name, Email = email };
            if (!p.Validate(out var errors))
                throw new InvalidOperationException(string.Join("; ", errors));

            db.Participants.Add(p);
            await db.SaveChangesAsync();
            return p;
        }

        public async Task UpdateAsync(int id, string name, string email)
        {
            using var db = await _factory.CreateDbContextAsync();

            var p = await db.Participants.FindAsync(id)
                ?? throw new InvalidOperationException("Osallistujaa ei löytynyt.");

            p.Name = name;
            p.Email = email;

            if (!p.Validate(out var errors))
                throw new InvalidOperationException(string.Join("; ", errors));

            var dupe = await db.Participants
                .AnyAsync(x => x.Id != id && x.EventId == p.EventId && x.Email == p.Email);
            if (dupe)
                throw new InvalidOperationException("Sähköposti on jo ilmoitettu tähän tapahtumaan.");

            await db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            using var db = await _factory.CreateDbContextAsync();

            var p = await db.Participants.FindAsync(id)
                ?? throw new InvalidOperationException("Osallistujaa ei löytynyt.");

            db.Participants.Remove(p);
            await db.SaveChangesAsync();
        }
    }
}