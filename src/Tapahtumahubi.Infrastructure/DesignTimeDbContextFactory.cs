using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Tapahtumahubi.Infrastructure;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>();

        // Käytetään paikallista polkua design-tilassa (ei MAUI FileSystemiä)
        var dbPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "events.db");

        options.UseSqlite($"Data Source={dbPath}");
        return new AppDbContext(options.Options);
    }
}
