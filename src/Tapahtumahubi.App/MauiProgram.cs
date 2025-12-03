using System.IO;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Tapahtumahubi.Infrastructure;
using Tapahtumahubi.App.ViewModels;

namespace Tapahtumahubi.App
{
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

            // --- Serilog tiedostoloki ---
            var baseDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Tapahtumahubi.App");
            var logsDir = Path.Combine(baseDir, "logs");
            Directory.CreateDirectory(logsDir);
            var logFile = Path.Combine(logsDir, "app-.log");

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .Enrich.FromLogContext()
#if DEBUG
                .WriteTo.Debug()
#endif
                .WriteTo.File(
                    path: logFile,
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 7,
                    shared: true)
                .CreateLogger();

            builder.Logging.ClearProviders();
            builder.Logging.AddSerilog(dispose: true);

            // --- Palvelut / DI ---
            Directory.CreateDirectory(baseDir);
            var dbPath = Path.Combine(baseDir, "app.db");

            builder.Services.AddDbContextFactory<AppDbContext>(opt =>
            {
                opt.UseSqlite($"Data Source={dbPath}");
            });

            // ViewModelit
            builder.Services.AddTransient<MainPageViewModel>();
            builder.Services.AddTransient<NewEventPageViewModel>();
            builder.Services.AddTransient<CalendarPageViewModel>();

            // Sivut
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<NewEventPage>();
            builder.Services.AddTransient<CalendarPage>();
            builder.Services.AddTransient<ParticipantsPage>();
            builder.Services.AddTransient<AddEditParticipantPage>();

            // Sovelluspalvelut
            builder.Services.AddScoped<IParticipantService, ParticipantService>();

            return builder.Build();
        }
    }
}