using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Storage;
using Tapahtumahubi.Infrastructure;
using Tapahtumahubi.App.ViewModels;

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

#if DEBUG
        // Kirjoita frameworkin logit VS:n/Debug Outputiin
        builder.Logging.ClearProviders();
        builder.Logging.AddDebug();
#endif

        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "events.db");
        builder.Services.AddDbContextFactory<AppDbContext>(opt => opt.UseSqlite($"Data Source={dbPath}"));

        // Pages
        builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<NewEventPage>();
        builder.Services.AddTransient<EditEventPage>();
        builder.Services.AddSingleton<IParticipantService, ParticipantService>();
        builder.Services.AddTransient<ParticipantsPage>();
        builder.Services.AddTransient<AddEditParticipantPage>();

        // ViewModels
        builder.Services.AddTransient<MainPageViewModel>();
        builder.Services.AddTransient<NewEventPageViewModel>();

        var app = builder.Build();

        using var scope = app.Services.CreateScope();
        var factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<AppDbContext>>();
        using var db = factory.CreateDbContext();
        db.Database.Migrate();

#if DEBUG
        System.Diagnostics.Debug.WriteLine($"[DB PATH] {dbPath}");
#endif

        return app;
    }
}