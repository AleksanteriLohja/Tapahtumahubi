using System.IO;
using Microsoft.Extensions.Logging;
using Serilog;

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

        // ---- Serilog: tiedostoloki + (DEBUGissä) VS Output/Debug ----
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
            .WriteTo.Debug() // vaatii Serilog.Sinks.Debug
#endif
            .WriteTo.File(
                path: logFile,
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 7,
                shared: true)
            .CreateLogger();

        builder.Logging.ClearProviders();
        builder.Logging.AddSerilog(dispose: true);

        // …palvelurekisteröinnit jos tarvitset (DbContext, Services, jne.)

        return builder.Build();
    }
}
