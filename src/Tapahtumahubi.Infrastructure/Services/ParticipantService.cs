using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        private readonly AppDbContext _db;
        public ParticipantService(AppDbContext db) => _db = db;

        public async Task<List<Participant>> ListByEventAsync(int eventId)
        {
            return await _db.Participants
                .AsNoTracking()
                .Where(p => p.EventId == eventId)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<Participant> AddAsync(int eventId, string name, string email)
        {
            var ev = await _db.Events
                .Include(e => e.Participants)
                .SingleOrDefaultAsync(e => e.Id == eventId)
                ?? throw new InvalidOperationException("Tapahtumaa ei löytynyt.");

            if (ev.Participants.Count >= ev.MaxParticipants)
                throw new InvalidOperationException("Tapahtuma on täynnä.");

            var duplicate = await _db.Participants.AnyAsync(x => x.EventId == eventId && x.Email == email);
            if (duplicate)
                throw new InvalidOperationException("Sähköposti on jo ilmoitettu tähän tapahtumaan.");

            var p = new Participant { EventId = eventId, Name = name, Email = email };
            if (!p.Validate(out var errors))
                throw new InvalidOperationException(string.Join("; ", errors));

            _db.Participants.Add(p);
            await _db.SaveChangesAsync();
            return p;
        }

        public async Task UpdateAsync(int id, string name, string email)
        {
            var p = await _db.Participants.FindAsync(id)
                ?? throw new InvalidOperationException("Osallistujaa ei löytynyt.");

            p.Name = name;
            p.Email = email;

            if (!p.Validate(out var errors))
                throw new InvalidOperationException(string.Join("; ", errors));

            var dupe = await _db.Participants
                .AnyAsync(x => x.Id != id && x.EventId == p.EventId && x.Email == p.Email);
            if (dupe)
                throw new InvalidOperationException("Sähköposti on jo ilmoitettu tähän tapahtumaan.");

            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var p = await _db.Participants.FindAsync(id)
                ?? throw new InvalidOperationException("Osallistujaa ei löytynyt.");

            _db.Participants.Remove(p);
            await _db.SaveChangesAsync();
        }
    }
}