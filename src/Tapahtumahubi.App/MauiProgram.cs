using Microsoft.EntityFrameworkCore;
using Tapahtumahubi.Infrastructure;

namespace Tapahtumahubi.App;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "events.db");

        builder.Services.AddDbContextFactory<AppDbContext>(opt =>
            opt.UseSqlite($"Data Source={dbPath}"));

        builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<NewEventPage>();
        builder.Services.AddTransient<EditEventPage>();

        var app = builder.Build();

        using var scope = app.Services.CreateScope();
        var factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<AppDbContext>>();
        using var db = factory.CreateDbContext();

        // (Oikea tapa) – suorita odottavat migraatiot
        db.Database.Migrate();

#if DEBUG
        System.Diagnostics.Debug.WriteLine($"[DB PATH] {dbPath}");
#endif

        return app;
    }
}