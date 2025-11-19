using Microsoft.EntityFrameworkCore;
using Tapahtumahubi.Domain;

namespace Tapahtumahubi.Infrastructure;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Event> Events => Set<Event>();
    public DbSet<Participant> Participants => Set<Participant>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        base.OnModelCreating(b);

        b.Entity<Event>(e =>
        {
            e.Property(x => x.Title).IsRequired().HasMaxLength(200);
            e.Property(x => x.Location).IsRequired().HasMaxLength(200);
            e.Property(x => x.MaxParticipants).HasDefaultValue(50);

            e.HasMany(x => x.Participants)
             .WithOne()
             .HasForeignKey(p => p.EventId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        b.Entity<Participant>(p =>
        {
            p.Property(x => x.Name).IsRequired().HasMaxLength(200);
            p.Property(x => x.Email).IsRequired().HasMaxLength(200);

            // Estä sama sähköposti samassa tapahtumassa
            p.HasIndex(x => new { x.EventId, x.Email }).IsUnique();
        });
    }
}